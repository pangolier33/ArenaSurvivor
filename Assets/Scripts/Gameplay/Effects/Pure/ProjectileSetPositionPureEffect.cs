using System;
using Bones.Gameplay.Effects.Containers.Projectiles;
using Bones.Gameplay.Effects.Provider;
using UnityEngine;

namespace Bones.Gameplay.Effects.Pure
{
    [Serializable]
    public sealed class ProjectileSetPositionPureEffect : GetVariablePureEffect<Vector2>
    {
        protected override void Invoke(ITrace trace, Vector2 value)
        {
            var handle = trace.Get<IProjectileHandle>();
            var handleTransform = handle.Instance.transform;
            handleTransform.position = value;
        }
    }
}