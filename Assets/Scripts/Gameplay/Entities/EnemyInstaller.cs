using System.Collections.Generic;
using System.Linq;
using Bones.Gameplay.Factories;
using Bones.Gameplay.Factories.Pools.Base;
using Bones.Gameplay.Flocks.Pulling;
using Bones.Gameplay.Items;
using Bones.Gameplay.Startup;
using UniRx;
using UnityEngine;
using Zenject;

namespace Bones.Gameplay.Entities
{
	public class EnemyInstaller : MonoInstaller
    {
	    [SerializeField] private float _cacheExpirationTime = 1;
        [SerializeField] private Vector2Int _extents;
        [SerializeField] private float _clusterSize;
        [SerializeField] private ItemCollectingSettings _itemSettings;
        [SerializeField] private Material _hitMaterial;
        [SerializeField] private Material _defaultMaterial;
		[SerializeField] private Material _deathMaterial;
		[SerializeField] private EnemyUpdater _updater;
        
        public override void InstallBindings()
        {
            Container.BindInstance(_hitMaterial).WithId(MaterialId.EnemyHit).WhenInjectedInto<Enemy>();
            Container.BindInstance(_defaultMaterial).WithId(MaterialId.EnemyDefault).WhenInjectedInto<Enemy>();
			Container.BindInstance(_deathMaterial).WithId(MaterialId.EnemyDeath).WhenInjectedInto<Enemy>();

			Container.Bind<EnemiesCollectionCache>()
                     .AsSingle()
                     .WithArguments(_cacheExpirationTime)
                     .Lazy();

			Container.Bind<KillCounter>()
				.AsSingle()
				.NonLazy();
            
            var broker = new MessageBroker();
            Container.Bind<IMessageReceiver>().FromInstance(broker);
            Container.Bind<IMessagePublisher>().FromInstance(broker);

            Container.BindIFactory<Enemy, SingletonEnemyAnimator>()
                     .FromMethod(CreateEnemyAnimator)
                     .WhenInjectedInto<CachedFactory<Enemy, SingletonEnemyAnimator>>()
                     .Lazy();

            Container.BindInterfacesTo<CachedFactory<Enemy, SingletonEnemyAnimator>>()
                     .AsSingle()
                     .Lazy();

            Container.BindIFactory<Enemy, IPool<Enemy.Data, Enemy>>()
                     .FromMethod((container, enemy) => new Enemy.Pool(new Factories.PrefabFactory<Enemy>(container, enemy)))
                     .WhenInjectedInto<CachedFactory<Enemy, IPool<Enemy.Data, Enemy>>>();

            Container.BindInterfacesAndSelfTo<CachedFactory<Enemy, IPool<Enemy.Data, Enemy>>>()
                .AsSingle();

            Container.BindInterfacesTo<EnemyUpdater>()
                .FromInstance(_updater);
            
            Container.Bind<IEnumerable<Enemy>>()
                .FromResolveGetter<CachedFactory<Enemy, IPool<Enemy.Data, Enemy>>>(ExtractEnemies)
                .AsSingle();

            Container.BindIFactory<Item, IPool<Item>>()
                .FromMethod((container, item) => new Item.Pool(new Factories.PrefabFactory<Item>(container, item)))
                .WhenInjectedInto<CachedFactory<Item, IPool<Item>>>();

            Container.BindInterfacesAndSelfTo<CachedFactory<Item, IPool<Item>>>()
                .AsSingle();

            Container.Bind<IEnumerable<Item>>()
                .FromResolveGetter<CachedFactory<Item, IPool<Item>>>(ExtractItems)
                .AsSingle();

            Container.Bind<ItemsContainer>()
                .AsSingle()
                .WithArguments(_extents, _clusterSize, _itemSettings);

            Container.BindInstance(_itemSettings);
        }

        private static IEnumerable<Enemy> ExtractEnemies(CachedFactory<Enemy, IPool<Enemy.Data, Enemy>> arg)
        {
            return arg.Products.SelectMany(x => x.Entities);
        }
        
        private static IEnumerable<Item> ExtractItems(CachedFactory<Item, IPool<Item>> arg)
        {
            return arg.Products.SelectMany(x => x.Entities);
        }

        private static SingletonEnemyAnimator CreateEnemyAnimator(IInstantiator container, Enemy enemy)
        {
	        var @params = new object[]
	        {
		        enemy.SizeCurve,
		        enemy.transform.localScale.y
	        };
	        return container.Instantiate<SingletonEnemyAnimator>(@params);
        } 
    }
}