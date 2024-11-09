using System;
using Bones.Gameplay.Effects.Transitive.Depth;
using Bones.Gameplay.Players;
using Bones.Gameplay.Stats.Containers;
using Bones.Gameplay.Stats.Units;
using Railcar.Dice;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Bones.Gameplay
{
	[Serializable]
	public class SerializablePlayerStatsConfiguration : PlayerStatsConfigurationBase
	{
		private const int LabelWidth = 100;
		[SerializeField] [InlineProperty, LabelWidth(LabelWidth), BoxGroup("7", showLabel: false)] private ValueBuilder _health;
		[SerializeField] [InlineProperty, LabelWidth(LabelWidth), BoxGroup("8", showLabel: false)] private TimedValueBuilder _healthRegen;
		[SerializeField] [InlineProperty, LabelWidth(LabelWidth), BoxGroup("1", showLabel: false)] private ValueBuilder _shield;
		[SerializeField] [InlineProperty, LabelWidth(LabelWidth), BoxGroup("2", showLabel: false)] private TimedValueBuilder _shieldRegen;
		[SerializeField] [InlineProperty, LabelWidth(LabelWidth), BoxGroup("3", showLabel: false)] private PointsBuilder _armor;
		[SerializeField] [InlineProperty, LabelWidth(LabelWidth), BoxGroup("4", showLabel: false)] private PointsBuilder _harmony;
		[SerializeField] [InlineProperty, LabelWidth(LabelWidth), BoxGroup("9", showLabel: false)] private ValueBuilder _damage;
		[SerializeField] [InlineProperty, LabelWidth(LabelWidth), BoxGroup("5", showLabel: false)] private PointsBuilder _criticalChance;
		[SerializeField] [InlineProperty, LabelWidth(LabelWidth), BoxGroup("10", showLabel: false)] private ValueBuilder _criticalMultiplier;
		[SerializeField] [InlineProperty, LabelWidth(LabelWidth), BoxGroup("6", showLabel: false)] private ValueBuilder _thorns;
		[Title("Cooldown")]
		[SuffixLabel("$CooldownDecrement")]
		[SerializeField] private int _cooldownDecrementValue;
		[SerializeField] private double _cooldownDecrementCoefficient;
		[SuffixLabel("$CooldownDuration")]
		[SerializeField] private float _cooldownBaseValue;
		[Title("Multicast")]
		[SuffixLabel("$MulticastChance")]
		[SerializeField] private int _multicastPoints;
		[SerializeField] private double _multicastCoefficient;
		[Title("Moving Speed")]
		[SerializeField] private float _movingSpeed;
		[SerializeField] private float _movingSpeedBoostingDelay;
		[SerializeField] private double _movingSpeedBoostingAmplifier;
		[Title("Stamina")]
		[SerializeField] private ValueBuilder _staminaMax;
		[SerializeField] private TimedValueBuilder _staminaRegenVelocity;
		[SerializeField] private TimedValueBuilder _staminaLossVelocity;
		[SerializeField] private float _staminaSpeedPenalty;
		[SerializeField] private float _staminaMinPartToUse;
		[SerializeField] private int _staminaKillEnemyScores;

		private IDice _dice;

		protected override Contract GetContract() => new(
			_health.BuildConcrete(), _health.Build(),
			_shield.BuildConcrete(), _shield.Build(),
			_healthRegen.BuildReadOnly(), _shieldRegen.BuildReadOnly(),
			new CooldownStat(new Value(_cooldownBaseValue), new Points(_cooldownDecrementValue, _cooldownDecrementCoefficient)),
			_armor.Build(), _harmony.Build(),
			_damage.Build(), _criticalChance.Build(), _criticalMultiplier.Build(),
			_thorns.Build(),
			new MultiChanceStat(new Points(_multicastPoints, _multicastCoefficient), _dice),
			new ReadOnlyStat<SpeedValue>(new SpeedValue { Amplifier = _movingSpeed }), _movingSpeedBoostingDelay, new Value(_movingSpeedBoostingAmplifier),
			_staminaMax.BuildConcrete(), _staminaRegenVelocity.Build(), _staminaLossVelocity.Build(),
				new ReadOnlyStat<StaminaValue>(
					new StaminaValue {
						maxScore = (int)_staminaMax.Build().BaseValue.Base,
						slowDownParameter = _staminaSpeedPenalty,
						minPartToUse = _staminaMinPartToUse,
						enemyKillScores = _staminaKillEnemyScores}
					),
			new Stat<bool>(false)
			);

		[Inject]
		private void Inject(IDice dice)
		{
			_dice = dice;
		}

		#if UNITY_EDITOR
		public float MulticastChance => (float)(1d - Math.Exp(-_multicastPoints * _multicastCoefficient));
		public float CooldownDuration => _cooldownBaseValue * (1 - CooldownDecrement);
		public float CooldownDecrement => (float)(1d - Math.Exp(-_cooldownDecrementValue * _cooldownDecrementCoefficient));
		#endif
	}
}