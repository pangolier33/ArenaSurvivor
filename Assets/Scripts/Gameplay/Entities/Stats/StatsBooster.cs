using System;
using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Effects.Transitive.Depth;
using Bones.Gameplay.Startup;
using Bones.Gameplay.Stats.Containers;
using Bones.Gameplay.Stats.Containers.Graphs;
using Bones.Gameplay.Stats.Units;
using Bones.Gameplay.Stats.Units.Operations;
using Bones.Gameplay.Waves;
using UnityEngine;

namespace Bones.Gameplay.Entities.Stats
{
	public sealed class StatsBooster
	{
		private IWaveProvider _waveProvider;
		private Curves _curves;
		private float _maxTime;

		private StatsBooster(
			IWaveProvider waveProvider,
			Curves curves,
			float maxTime)
		{
			_waveProvider = waveProvider;
			_curves = curves;
			_maxTime = maxTime;
		}

		public void Boost(IStatMap map)
		{
			var timeFactor = _waveProvider.GeneralTime;

			var healthBoost = new Value(_curves.HealthCurve.Evaluate(timeFactor));
			MultiplyStatRaw<Value, Value>(map.Get(StatName.Health), healthBoost);

			var damageBoost = new Value(_curves.DamageCurve.Evaluate(timeFactor));
			MultiplyStatRaw<Value, Value>(map.Get(StatName.Damage), damageBoost);
			
			var fakeDamageBoost = new Value(_curves.FakeDamageCurve.Evaluate(timeFactor));
			MultiplyStatRaw<Value, Value>(map.Get(StatName.FakeDamage), fakeDamageBoost);

			var armorBoost = new Value(_curves.ArmorCurve.Evaluate(timeFactor));
			MultiplyStatRaw<Points, Value>(map.Get(StatName.Armor), armorBoost);
		}

		private static void MultiplyStatRaw<T1, T2>(IStat stat, T2 multiplier) where T1 : IMultiplySupportable<T2, T1>
		{
			if (stat is not ReadOnlyStat<T1> rawStat)
				throw new InvalidCastException("Cannot access stat's base value");

			rawStat.BaseValue = rawStat.BaseValue.Multiply(multiplier);
		}

		[Serializable]
		public struct Curves
		{
			[field: SerializeField] public ScriptableCurve HealthCurve { get; private set; }
			[field: SerializeField] public ScriptableCurve DamageCurve { get; private set; }
			[field: SerializeField] public ScriptableCurve ArmorCurve { get; private set; }
			[field: SerializeField] public ScriptableCurve FakeDamageCurve { get; private set; }
		}
	}
}