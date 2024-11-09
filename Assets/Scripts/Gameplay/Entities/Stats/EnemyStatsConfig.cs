using System.Collections.Generic;
using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Stats.Containers;
using Bones.Gameplay.Stats.Containers.Builders;
using Bones.Gameplay.Stats.Containers.Graphs;
using Bones.Gameplay.Stats.Utils;
using Bones.Gameplay.Waves.Spawning.Amounts;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Bones.Gameplay.Entities.Stats
{
	[CreateAssetMenu(menuName = "Entities/Enemies/Stats", fileName = "New EnemyStats")]
	public class EnemyStatsConfig : ScriptableObject, IFactory<IStatMap>
	{
		[SerializeField, InlineProperty] private ValueBuilder _health;
		[SerializeField, InlineProperty] private ValueBuilder _damage;
		[SerializeField, InlineProperty] private ValueBuilder _attackDelay;
		[SerializeField, InlineProperty] private ValueBuilder _speed;
		
		[SerializeField, InlineProperty, BoxGroup] private PointsBuilder _harmony;
		[SerializeField, InlineProperty, BoxGroup("2", false)] private PointsBuilder _armor;
		[SerializeReference] public IAmountFResolver EnemySizeOffsetResolver = new RandomFAmountResolver(0, 0.2f);
		
		private DiContainer _container;
		
		public IStatMap Create()
		{
			var healthStat = _health.BuildConcrete();
			
			var armorStat = _armor.BuildReadOnly();
			healthStat.ModifyOnSubtract(value => new((1 - armorStat.Get().Percent) * value));
			
			var harmonyStat = _harmony.BuildReadOnly();
			healthStat.ModifyOnAdd(value => new((1 + harmonyStat.Get().Percent) * value));
			
			var map = new DictionaryStatsConfiguration.Adapter(new Dictionary<StatName, IStat>
			{
				{ StatName.Health, healthStat },
				{ StatName.Armor, _armor.BuildReadOnly() },
				{ StatName.Harmony, _harmony.BuildReadOnly() },
				{ StatName.Damage, _damage.BuildReadOnly() },
				//back enemy damage. lets get start with basic
				{ StatName.FakeDamage, _damage.BuildReadOnly() },
				{ StatName.AttackDelay, _attackDelay.BuildReadOnly() },
				{ StatName.MovingSpeed, _speed.BuildReadOnly() },
				{ StatName.OverallHealth, healthStat },
			});
			
			_container.Resolve<StatsBooster>().Boost(map);
			return map;
		}
		
		[Inject]
		private void Inject(DiContainer container)
		{
			_container = container;
		}
	}
}