using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Entities;
using Bones.Gameplay.Factories;
using Bones.Gameplay.Map;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Bones.Gameplay.Players
{
	public class PlayerInstaller : MonoInstaller
    {
        [SerializeField]
        [InlineEditor]
        private ScriptablePlayerStatsConfiguration _statsConfiguration;
        
        [SerializeField]
        private Player _player;
        
        [SerializeField]
        private Material _defaultMaterial;
        
        [SerializeField]
        private Material _hitMaterial;
        
        public override void InstallBindings()
        {
            Container.Bind<Player>()
                .FromComponentInNewPrefab(_player)
                .AsSingle()
                .NonLazy();

            Container.Bind<IStatMap>()
                .FromMethod(context =>
                {
                    context.Container.Inject(_statsConfiguration);
                    return _statsConfiguration.Create();
                })
                .AsCached()
                .Lazy();
            
            Container.BindInterfacesTo<PlayerPositionTracker>()
                .AsSingle()
                .Lazy();
            Container.BindInstance(_defaultMaterial).WithId(MaterialId.PlayerDefault).WhenInjectedInto<Player>();
            Container.BindInstance(_hitMaterial).WithId(MaterialId.PlayerHit).WhenInjectedInto<Player>();

			Container.BindIFactory<Player, SingletonEnemyAnimator>()
					 .FromMethod((container, player) => container.Instantiate<SingletonEnemyAnimator>(new object[] { player.SizeCurve, player.transform.localScale.y }))
					 .WhenInjectedInto<CachedFactory<Player, SingletonEnemyAnimator>>()
					 .Lazy();

			Container.BindInterfacesTo<CachedFactory<Player, SingletonEnemyAnimator>>()
					 .AsSingle()
					 .Lazy();

		}
	}
}