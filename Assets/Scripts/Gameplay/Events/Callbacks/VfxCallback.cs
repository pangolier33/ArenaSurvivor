using Bones.Gameplay.Events.Args;
using Bones.Gameplay.Factories.Pools.Base;
using JetBrains.Annotations;
using UnityEngine;

namespace Bones.Gameplay.Events.Callbacks
{
    public class VfxCallback<T> : BaseCallback<T>
        where T : IPositionalMessageArgs
    {
        private readonly IPool<ParticleSystem> _pool;

        public VfxCallback([NotNull] IPool<ParticleSystem> pool)
        {
            _pool = pool;
        }

        public override void OnNext(T args)
        {
            var system = _pool.Create();
            system.transform.position = args.Position;
        }
    }
}