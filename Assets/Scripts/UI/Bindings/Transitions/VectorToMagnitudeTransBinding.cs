using System;
using Bones.UI.Bindings.Transitions.Base;
using UnityEngine;

namespace Bones.UI.Bindings.Transitions
{
    [Serializable]
    public class VectorToMagnitudeTransBinding : TransBinding<Vector2, float>
    {
        protected override float Convert(Vector2 from)
        {
            return from.magnitude;
        }
    }
}