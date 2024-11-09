using System;
using UnityEngine;

namespace Bones.Gameplay.Effects.Transitive.Depth
{
    [Serializable]
    public sealed class VectorNormalizingTransitiveEffect : UnaryOperationTransitiveEffect<Vector2, Vector2>
    {
        protected override Vector2 Calculate(Vector2 arg)
        {
            return arg.normalized;
        }
    }
}