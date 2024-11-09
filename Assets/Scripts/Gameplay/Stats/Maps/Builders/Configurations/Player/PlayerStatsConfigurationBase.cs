using System;
using System.Collections.Generic;
using Bones.Gameplay.Effects.Transitive.Depth;
using Bones.Gameplay.Events.Callbacks.Templates.Base;
using Bones.Gameplay.Inputs;
using Bones.Gameplay.Players;
using Bones.Gameplay.Stats.Containers;
using Bones.Gameplay.Stats.Containers.Builders;
using Bones.Gameplay.Stats.Containers.Graphs;
using Bones.Gameplay.Stats.Numbers;
using Bones.Gameplay.Stats.Units;
using Bones.Gameplay.Stats.Utils;
using Bones.Gameplay.Utils;
using Railcar.Dice;
using Railcar.Time;
using UnityEngine;
using Zenject;

namespace Bones.Gameplay
{
	public abstract class PlayerStatsConfigurationBase : DictionaryStatsConfiguration, IInjectable
	{
		private IDirectionalInput _input;
		private IDice _dice;
		private IStopwatch _stopwatch;
		private ITimer _timer;
		protected override IDictionary<StatName, IStat> ToDictionary() => ToDictionary(GetContract(), _dice, _input, _stopwatch, _timer);
		protected abstract Contract GetContract();

		private static IDictionary<StatName, IStat> ToDictionary(Contract configuration, IDice dice, IDirectionalInput input, IStopwatch stopwatch, ITimer timer)
		{
			var isMovingSpeedBoosted = false;
			var movingSpeedBoostingTimeCap = 0f;
			stopwatch.Observe(timeDelta =>
			{
				if (input.Direction.Approximately(Vector2.zero))
				{
					movingSpeedBoostingTimeCap = 0;
					return;
				}

				movingSpeedBoostingTimeCap += timeDelta;
				if (movingSpeedBoostingTimeCap > configuration.MovingSpeedBoostingDelay)
					isMovingSpeedBoosted = true;
			});

			configuration.MovingSpeed.ModifyOnGet(x => isMovingSpeedBoosted switch {
				true => new SpeedValue { IsBoosted = true, Amplifier = x.Amplifier * configuration.MovingSpeedBoostingAmplifier },
				false => new SpeedValue { IsBoosted = false, Amplifier = x.Amplifier }
			});
			configuration.Damage.ModifyOnGet(value => value * (dice.Roll((float)configuration.CriticalChance.Get().Percent) ? configuration.CriticalMultiplier.Get() : Value.Unit));

			timer.MarkAndRepeat(() => configuration.HealthRegen.Get().Interval, _ =>
			{
				var clampedAmount = Math.Min(
					configuration.HealthRegen.Get().Amount,
					configuration.MaxHealth.Get() - configuration.Health.Get()
				);
				configuration.Health.Add(new Value(clampedAmount));
			});
			timer.MarkAndRepeat(() => configuration.ShieldRegen.Get().Interval, _ =>
			{
				var clampedAmount = Math.Min(
					configuration.ShieldRegen.Get().Amount,
					configuration.MaxShield.Get() - configuration.Shield.Get()
				);
				configuration.Shield.Add(new Value(clampedAmount));
			});
			configuration.Health.ModifyOnSet(value =>
			{
				var max = configuration.MaxHealth.Get();
				if (value > max)
				{
					configuration.Shield.Set(configuration.Shield.Get() + value - max);
					return max;
				}
				if (value < Value.Zero)
				{
					return Value.Zero;
				}
				return value;
			});
			configuration.Shield.ModifyOnSet(value =>
			{
				var max = configuration.MaxShield.Get();
				if (value > max)
				{
					return max;
				}
				if (value < Value.Zero)
				{
					configuration.Health.Set(configuration.Health.Get() + value);
					return Value.Zero;
				}
				return value;
			});

			timer.MarkAndRepeat(() => configuration.staminaLossVelocity.Get().Interval, _ =>
			{
				if (configuration.staminaZeroReached.Get()) return;
				if (input.Direction == Vector2.zero) return;
				
				var clampedAmountDec = configuration.CurrentStamina.Get() - configuration.staminaLossVelocity.Get().Amount;
				clampedAmountDec = clampedAmountDec < 0 ? configuration.CurrentStamina.Get() : configuration.staminaLossVelocity.Get().Amount;

				configuration.CurrentStamina.Subtract(new Value(clampedAmountDec));

				if (configuration.CurrentStamina.Get() <= 0)
					configuration.staminaZeroReached.Set(true);

				//Debug.Log("loss " + configuration.CurrentStamina.Get()); 
			});

			timer.MarkAndRepeat(() => configuration.staminaRegenVelocity.Get().Interval, _ =>
			{
				var staminaParametrs = configuration.StaminaParametrs.Get();
				var currentStamina = configuration.CurrentStamina.Get();

				staminaParametrs.Add(configuration.CurrentStamina, configuration.staminaRegenVelocity.Get().Amount, configuration.staminaZeroReached);

				//Debug.Log("regen " + currentStamina);

			});

			// TODO: route processors properly
			configuration.Health.ModifyOnAdd(value => new Value(1f + configuration.Harmony.Get().Percent) * value);
			configuration.Shield.ModifyOnSubtract(value => new Value(1f - configuration.Armor.Get().Percent) * value);

			return new Dictionary<StatName, IStat>
			{
				{ StatName.OverallHealth, new StatRoutingWrapper<Value>
										  {
											  AddStat = configuration.Health,
											  SubtractStat = configuration.Shield,
											  GetStat = new SummarisingStatWrapper<Value>
											  {
												  WrappedStats = new[]
												  {
													  configuration.Health,
													  configuration.Shield
												  }
											  }
										  }
				},
				{ StatName.Health, configuration.Health },
				{ StatName.MaxHealth, configuration.MaxHealth },
				{ StatName.Shield, configuration.Shield },
				{ StatName.MaxShield, configuration.MaxShield },

				{ StatName.HealthRegen, configuration.HealthRegen },
				{ StatName.ShieldRegen, configuration.ShieldRegen },

				{ StatName.Cooldown, configuration.Cooldown },

				{ StatName.CriticalChance, configuration.CriticalChance },
				{ StatName.CriticalMultiplier, configuration.CriticalMultiplier },
				{ StatName.Damage, configuration.Damage },
				{ StatName.Thorns, configuration.Thorns },

				{ StatName.Armor, configuration.Armor },
				{ StatName.Harmony, configuration.Harmony },

				{ StatName.MovingSpeed, configuration.MovingSpeed },
				{ StatName.Multicast, configuration.Multicast },

				{ StatName.Stamina, configuration.CurrentStamina },
				{ StatName.StaminaParametrs, configuration.StaminaParametrs },
				{ StatName.StaminaZeroReached, configuration.staminaZeroReached }
			};
		}

		[Inject]
		private void Inject(IDice dice, IDirectionalInput input, [Inject(Id = TimeID.Fixed)] IStopwatch stopwatch, [Inject(Id = TimeID.Fixed)] ITimer timer)
		{
			_input = input;
			_dice = dice;
			_stopwatch = stopwatch;
			_timer = timer;
		}

		protected record Contract(
			UncertainStat<Value, Value, Value> Health, Stat<Value> MaxHealth,
			UncertainStat<Value, Value, Value> Shield, Stat<Value> MaxShield,
			ReadOnlyStat<TimedValue> HealthRegen, ReadOnlyStat<TimedValue> ShieldRegen,
			CooldownStat Cooldown,
			Stat<Points> Armor, Stat<Points> Harmony,
			Stat<Value> Damage, Stat<Points> CriticalChance, Stat<Value> CriticalMultiplier,
			Stat<Value> Thorns,
			MultiChanceStat Multicast,
			ReadOnlyStat<SpeedValue> MovingSpeed, float MovingSpeedBoostingDelay, float MovingSpeedBoostingAmplifier,
			UncertainStat<Value, Value, Value> CurrentStamina, Stat<TimedValue> staminaRegenVelocity, Stat<TimedValue> staminaLossVelocity,
			ReadOnlyStat<StaminaValue> StaminaParametrs, Stat<bool> staminaZeroReached);
	}
}