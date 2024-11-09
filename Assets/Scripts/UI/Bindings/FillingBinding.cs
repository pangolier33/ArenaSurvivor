using System;
using Bones.UI.Bindings.Base;
using UnityEngine;
using UnityEngine.UI;

namespace Bones.UI.Bindings
{
    [Serializable]
    public class FillingBinding : BaseBinding<float>
    {
        [SerializeField] private Image _image;
        
        public override void OnNext(float value)
        {
            _image.fillAmount = value;
        }
    }
}