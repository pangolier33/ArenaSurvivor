using Bones.Gameplay.Effects.Containers.Projectiles;
using Bones.Gameplay.Factories;
using Bones.Gameplay.Factories.Pools.Base;
using Zenject;

namespace Bones.Gameplay.Effects.Containers
{
    public class EffectsInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindIFactory<ProjectileEffectContainer, IPool<ProjectileSettings, ProjectileEffectContainer>>()
                .FromFactory<CachedFactory<ProjectileEffectContainer, IPool<ProjectileSettings, ProjectileEffectContainer>>>();
            Container.BindIFactory<ProjectileEffectContainer, IPool<ProjectileSettings, ProjectileEffectContainer>>()
                .FromMethod(CreatePool)
                .WhenInjectedInto<CachedFactory<ProjectileEffectContainer, IPool<ProjectileSettings, ProjectileEffectContainer>>>();
        }

        private IPool<ProjectileSettings, ProjectileEffectContainer> CreatePool(DiContainer container, ProjectileEffectContainer prefab)
        {
            return container.Instantiate<ProjectileEffectContainer.Pool>(new []
            {
                container.Instantiate<Bones.Gameplay.Factories.PrefabFactory<ProjectileEffectContainer>>(new []
                {
                    prefab
                })
            });
        }
    }
}