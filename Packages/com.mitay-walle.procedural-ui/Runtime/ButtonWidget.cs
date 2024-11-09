using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Mitaywalle.ProceduralUI
{
    public class ButtonWidget : LabeledWidget
    {
        [SerializeField] private Button _button;

        public void SetAction(Action onClick)
        {
            if (onClick == null) return;

            var action = new UnityAction(onClick);
            _button.onClick.RemoveListener(action);
            _button.onClick.AddListener(action);
        }
        public TMP_Text GetLabel() => _label;
    }
}
