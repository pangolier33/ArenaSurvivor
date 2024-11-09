using System;
using Bones.UI.Bindings.Base;
using UnityEngine;
using UnityEngine.UI;

namespace Bones.UI.Bindings
{
    [Serializable]
    public class PaintingBinding : BaseBinding<Color>
    {
        [SerializeField] private Graphic _image;
        
        public override void OnNext(Color value)
        {
            _image.color = value;
        }
    }
}