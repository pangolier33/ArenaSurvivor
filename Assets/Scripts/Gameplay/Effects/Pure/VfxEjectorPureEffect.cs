using System;
using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Factories.Pools.Base;
using UnityEngine;
using Zenject;

namespace Bones.Gameplay.Effects.Pure
{
    [Serializable]
    public sealed class VfxEjectorPureEffect : PureEffect
    {
        [SerializeField] private ParticleSystem _prefab;

        private IPool<ParticleSystem> _pool;

        protected override void Invoke(ITrace trace)
        {
            _pool.Create();
        }

        [Inject]
        private void Inject(IFactory<ParticleSystem, IPool<ParticleSystem>> factory)
        {
            _pool = factory.Create(_prefab);
        }
    }
}