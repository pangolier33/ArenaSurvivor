using System;
using Bones.UI.Bindings.Transitions.Base;
using UnityEngine;

namespace Bones.UI.Bindings.Transitions
{
    [Serializable]
    public class EvaluatingTransBinding : TransBinding<float, float>
    {
        [SerializeField] private AnimationCurve _curve;
        
        protected override float Convert(float from)
        {
            return _curve.Evaluate(from);
        }
    }
}