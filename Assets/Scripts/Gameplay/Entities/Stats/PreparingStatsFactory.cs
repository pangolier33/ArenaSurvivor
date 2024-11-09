using Zenject;

namespace Bones.Gameplay.Entities.Stats
{
	public sealed class PreparingStatsFactory : BaseStatsFactoryWrapper
	{
		private IFactory<EnemyStats, EnemyStatsProcessor> _wrappedFactory;
        
		public PreparingStatsFactory(IFactory<EnemyStats, EnemyStatsProcessor> wrappedFactory)
			: base(wrappedFactory)
		{
		}

		protected override EnemyStats Process(EnemyStats stats) => new()
		{
			Health = stats.Health,
			Harmony = 1f + stats.Harmony / 100f,
			Damage = stats.Damage,
			Armor = 1f - stats.Armor / 100f,
			CriticalChance = stats.CriticalChance / 100f,
			CriticalMultiplier = 1f + stats.CriticalMultiplier
		};
	}
}