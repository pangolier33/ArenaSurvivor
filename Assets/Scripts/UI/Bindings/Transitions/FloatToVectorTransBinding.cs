using System;
using Bones.UI.Bindings.Transitions.Base;
using UnityEngine;

namespace Bones.UI.Bindings.Transitions
{
    [Serializable]
    public class FloatToVectorTransBinding : TransBinding<float, Vector2>
    {
        [SerializeField] private Vector2 _multiplier;
        [SerializeField] private Vector2 _offset;
        
        protected override Vector2 Convert(float from)
        {
            return _multiplier * from + _offset;
        }
    }
}