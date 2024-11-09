using System;
using Bones.Gameplay.Entities;
using Bones.Gameplay.Events.Args;
using Bones.Gameplay.Events.Callbacks.Templates.Base;
using Bones.Gameplay.Factories;
using Bones.Gameplay.Factories.Pools;
using Bones.Gameplay.Factories.Pools.Base;
using UniRx;
using UnityEngine;
using Zenject;

namespace Bones.Gameplay.Events
{
    public class MonoSubscriber : MonoInstaller, IInitializable, IDisposable
    {
        private readonly CompositeDisposable _subscriptions = new();
    
        [SerializeReference] private ICallbackTemplate[] _callbackTemplates;

        private IMessageReceiver _receiver;
    
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<MonoSubscriber>().FromInstance(this);
            Container.BindInterfacesTo<AudioPool>().AsSingle();
			Container.BindInterfacesTo<DamagePool>().AsSingle();

			Container.BindIFactory<ParticleSystem, IPool<ParticleSystem>>()
                .FromFactory<CachedFactory<ParticleSystem, IPool<ParticleSystem>>>();
            Container.BindIFactory<ParticleSystem, IPool<ParticleSystem>>()
                .FromMethod(((container, system) => container.Instantiate<ParticlePool>(new []{ container.Instantiate<Factories.PrefabFactory<ParticleSystem>>(new []{ system})})))
                .WhenInjectedInto<CachedFactory<ParticleSystem, IPool<ParticleSystem>>>();

			Container.BindIFactory<Damage, IPool<EnemyDamagedArgs, Damage>>()
				.FromFactory<CachedFactory<Damage, IPool<EnemyDamagedArgs, Damage>>>();
			Container.BindIFactory<Damage, IPool<EnemyDamagedArgs, Damage>>()
				.FromMethod(((container, damage) => container.Instantiate<DamagePool>(new[] { container.Instantiate<Factories.PrefabFactory<Damage>>(new[] { damage }) })))
				.WhenInjectedInto<CachedFactory<Damage, IPool<EnemyDamagedArgs, Damage>>>();

			foreach (var template in _callbackTemplates)
            {
                if (template is IInjectable)
                    Container.QueueForInject(template);
            }
        }
    
        public void Initialize()
        {
            foreach (var template in _callbackTemplates)
                template.Subscribe(_receiver);
        }

        public void Dispose()
        {
            _subscriptions.Dispose();
        }

        [Inject]
        private void Inject(IMessageReceiver receiver)
        {
            _receiver = receiver;
        }
    }
}