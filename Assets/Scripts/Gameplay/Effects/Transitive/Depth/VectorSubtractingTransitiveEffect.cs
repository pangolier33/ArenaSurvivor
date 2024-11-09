using System;
using UnityEngine;

namespace Bones.Gameplay.Effects.Transitive.Depth
{
    [Serializable]
    public sealed class VectorSubtractingTransitiveEffect : BinaryOperationTransitiveEffect<Vector2, Vector2, Vector2>
    {
        protected override Vector2 Calculate(Vector2 arg1, Vector2 arg2)
        {
            return arg1 - arg2;
        }
    }
}