using System;
using UnityEngine;

namespace Bones.Gameplay.Effects.Transitive.Depth
{
    [Serializable]
    public sealed class VectorToAngleTransitiveEffect : UnaryOperationTransitiveEffect<Vector2, float>
    {
        protected override float Calculate(Vector2 value)
        {
            return Vector2.SignedAngle(Vector2.up, value);
        }
    }
}