using Bones.Gameplay.Events.Args;
using Bones.Gameplay.Factories.Pools;
using Bones.Gameplay.Factories.Pools.Base;
using JetBrains.Annotations;
using UnityEngine;

namespace Bones.Gameplay.Events.Callbacks
{
    public class AudioCallback<T> : BaseCallback<T>
        where T : IPositionalMessageArgs
    {
        private readonly IPool<AudioInfo, AudioSource> _pool;
        private readonly AudioInfo _clip;

        public AudioCallback([NotNull] IPool<AudioInfo, AudioSource> pool, AudioInfo clip)
        {
            _pool = pool;
            _clip = clip;
        }

        public override void OnNext(T args)
        {
            var source = _pool.Create(_clip);
            source.transform.position = args.Position;
        }
    }
}