using System;
using UnityEngine;
using Zenject;

namespace Bones.Gameplay.Entities.Stats
{
	public sealed class ClampingStatsFactory : BaseStatsFactoryWrapper
	{
		private readonly StatsKit _kit;
        
		public ClampingStatsFactory(StatsKit kit, IFactory<EnemyStats, EnemyStatsProcessor> wrappedFactory)
			: base(wrappedFactory)
		{
			_kit = kit;
		}
        
		protected override EnemyStats Process(EnemyStats stats) => new()
		{
			Health = stats.Health,
			Harmony = _kit.Harmony.Process(stats.Harmony),
			Damage = stats.Damage,
			Armor = _kit.Armor.Process(stats.Armor),
			CriticalChance = _kit.CriticalChance.Process(stats.CriticalChance),
			CriticalMultiplier = _kit.CriticalMultiplier.Process(stats.CriticalMultiplier)
		};

		[Serializable]
		public struct ClampedStat
		{
			[SerializeField] private float _min;
			[SerializeField] private float _max;

			public float Process(float value)
			{
				return Mathf.Min(_max, Mathf.Max(_min, value));
			}
		}
        
		[Serializable]
		public struct StatsKit
		{
			[field: SerializeField] public ClampedStat Harmony { get; private set; }
			[field: SerializeField] public ClampedStat Armor { get; private set; }
			[field: SerializeField] public ClampedStat CriticalChance { get; private set; }
			[field: SerializeField] public ClampedStat CriticalMultiplier { get; private set; }
		}
	}
}