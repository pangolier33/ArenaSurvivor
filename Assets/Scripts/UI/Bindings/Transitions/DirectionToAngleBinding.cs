using System;
using Bones.UI.Bindings.Transitions.Base;
using UnityEngine;

namespace Bones.UI.Bindings.Transitions
{
    [Serializable]
    public sealed class DirectionToAngleBinding : TransBinding<Vector2, float>
    {
        protected override float Convert(Vector2 from)
        {
            return Vector2.SignedAngle(Vector2.up, from);
        }
    }
}