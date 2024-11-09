using System;
using UnityEngine;
using UnityEngine.UI;

namespace Mitaywalle.ProceduralUI
{
    public class ToggleWidget : LabeledWidget
    {
        [SerializeField] private Toggle _toggle;
        public Toggle GetTarget() => _toggle;

        public void SetAction(Action<bool> onValueChanged)
        {
            void Action(bool value)
            {
                onValueChanged?.Invoke(value);
            }

            _toggle.onValueChanged.AddListener(Action);
        }
    }
}
