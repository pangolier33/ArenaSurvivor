using System;
using Bones.UI.Bindings.Transitions.Base;
using UnityEngine;

namespace Bones.UI.Bindings.Transitions
{
    [Serializable]
    public sealed class GradientTransBinding : TransBinding<float, Color>
    {
        [SerializeField] private Gradient _gradient;
        
        protected override Color Convert(float from)
        {
            return _gradient.Evaluate(from);
        }
    }
}