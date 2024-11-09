using Zenject;

namespace Bones.Gameplay.Entities.Stats
{
	public abstract class BaseStatsFactoryWrapper : IFactory<EnemyStats, EnemyStatsProcessor>
	{
		private readonly IFactory<EnemyStats, EnemyStatsProcessor> _wrappedFactory;

		protected BaseStatsFactoryWrapper(IFactory<EnemyStats, EnemyStatsProcessor> wrappedFactory)
		{
			_wrappedFactory = wrappedFactory;
		}

		public EnemyStatsProcessor Create(EnemyStats param)
		{
			var stats = Process(param);
			return _wrappedFactory.Create(stats);
		}

		protected abstract EnemyStats Process(EnemyStats stats);
	}
}