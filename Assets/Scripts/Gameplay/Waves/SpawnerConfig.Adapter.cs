using Bones.Gameplay.Entities;
using Bones.Gameplay.Factories.Pools.Base;
using Bones.Gameplay.Items;
using Bones.Gameplay.Waves.Spawning.Positions;
using Zenject;

namespace Bones.Gameplay.Waves
{
	public partial class SpawnerConfig
	{
		// TODO: move to outer scope, make private properties accessible by interface?
		public class Adapter
		{
			private readonly DiContainer _container;
			private readonly IFactory<Enemy, IPool<Enemy.Data, Enemy>> _enemyPoolFactory;
			private readonly IFactory<Item, IPool<Item>> _itemPoolFactory;
			private readonly IFactory<PositionOriginName, IPositionResolver, IPositionResolver> _positionFactory;
			
			private Adapter(
				[Inject] DiContainer container,
				[Inject] IFactory<Enemy, IPool<Enemy.Data, Enemy>> enemyPoolFactory,
				[Inject] IFactory<Item, IPool<Item>> itemPoolFactory,
				[Inject] IFactory<PositionOriginName, IPositionResolver, IPositionResolver> positionFactory)
			{
				_container = container;
				_enemyPoolFactory = enemyPoolFactory;
				_itemPoolFactory = itemPoolFactory;
				_positionFactory = positionFactory;
			}
			
			public DraftSpawner.Settings Translate(SpawnerConfig config)
			{
				config._dropConfig.SetPoolFactory(_itemPoolFactory);
				_container.Inject(config._statsConfig);
				
				PositionOriginName origin = config._sharedConfig ? config._sharedConfig.Origin : config._origin;
				IPositionResolver position = config._sharedConfig ? config._sharedConfig.PositionResolver : config._positionResolver;
				
				return new DraftSpawner.Settings
				{
					Pool = _enemyPoolFactory.Create(config._prefab),
					EntitySettings = new Enemy.Data
					{
						DespawningDistance = config._sharedConfig ? config._sharedConfig.DespawningDistance : config._despawningDistance,
						StatsFactory = config._statsConfig,
						DropConfig = config._dropConfig,
						SizeOffsetResolver = config._statsConfig.EnemySizeOffsetResolver,
					},
					AmountResolver = config._amountResolver,
					Delay = config._spawningDelay,
					IsRepeating = config._isRepeating,
					Limit = config._limit,
					PositionResolver = _positionFactory.Create(origin, position),
				};
			}
		}
	}
}