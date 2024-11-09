using System;
using Bones.Gameplay.Effects.Containers.Projectiles;
using Bones.Gameplay.Effects.Provider;
using UnityEngine;

namespace Bones.Gameplay.Effects.Pure
{
    [Serializable]
    public sealed class ProjectileRotatingPureEffect : GetVariablePureEffect<float>
    {
        protected override void Invoke(ITrace trace, float value)
        {
            var handle = trace.Get<IProjectileHandle>();
            var handleTransform = handle.Instance.transform;
            handleTransform.Rotate(Vector3.forward, value, Space.World);
        }
    }
}