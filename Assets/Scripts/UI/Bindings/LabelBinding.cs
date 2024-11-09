using System;
using Bones.UI.Bindings.Base;
using TMPro;
using UnityEngine;

namespace Bones.UI.Bindings
{
    [Serializable]
    public class LabelBinding : BaseBinding<string>
    {
        [SerializeField] private TMP_Text _label;
        
        public LabelBinding() {}

        public LabelBinding(TMP_Text label)
        {
            _label = label;
        }

        public override void OnNext(string value)
        {
            _label.text = value;
        }
    }
}