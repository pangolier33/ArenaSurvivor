using System;
using Bones.Gameplay.Factories.Pools.Base;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Bones.Gameplay.Factories.Pools
{
	public sealed class ParticlePool : BasePool<ParticleSystem>, IDisposable
    {
        private readonly IFactory<ParticleSystem> _factory;
        private readonly IDisposable _timeSubscription;

        public ParticlePool(IFactory<ParticleSystem> factory)
        {
            _factory = factory;
        }

        public void Dispose()
        {
            _timeSubscription.Dispose();
        }
        protected override ParticleSystem Instantiate()
        {
            var system = _factory.Create();
            return system;
        }

        protected override async void OnSpawned(ParticleSystem entity)
        {
            entity.Play();
            await UniTask.Delay((int)(entity.main.duration * 1000));
            Despawn(entity);
        }

        protected override void OnDespawned(ParticleSystem entity)
        {
        }
    }
}