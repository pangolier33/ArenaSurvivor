using System;
using Bones.Gameplay.Effects.Containers.Projectiles;
using Bones.Gameplay.Effects.Provider;
using UnityEngine;

namespace Bones.Gameplay.Effects.Pure
{
    [Serializable]
    public class ProjectileSetScalePureEffect : GetVariablePureEffect<Vector2>
    {
        protected override void Invoke(ITrace trace, Vector2 value)
        {
            trace.Get<IProjectileHandle>().Instance.transform.localScale = new Vector3(value.x, value.y, 1);
        }
    }
}