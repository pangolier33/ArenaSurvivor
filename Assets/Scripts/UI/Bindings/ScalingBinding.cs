using System;
using Bones.UI.Bindings.Base;
using UnityEngine;

namespace Bones.UI.Bindings.Transitions
{
    [Serializable]
    public class ScalingBinding : BaseBinding<Vector2>
    {
        [SerializeField] private Transform _target; 

        public override void OnNext(Vector2 value)
        {
            _target.localScale = new Vector3(value.x, value.y, 1);
        }
    }
}