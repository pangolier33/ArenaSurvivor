using System;
using Bones.UI.Bindings.Base;
using UnityEngine;

namespace Bones.UI.Bindings
{
    [Serializable]
    public sealed class RotatingBinding : BaseBinding<float>
    {
        [SerializeField] private Transform _subject;
        
        public override void OnNext(float value)
        {
            _subject.rotation = Quaternion.Euler(0, 0, value);
        }
    }
}