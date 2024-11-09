using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mitaywalle.ProceduralUI
{
    public class SliderWidget : Widget
    {
        [SerializeField] private Slider _slider;
        [SerializeField] private TMP_Text _labelValue;
        [SerializeField] private string _format = "{0}";
        [SerializeField] private string _valueFormat;

        public Slider GetTarget() => _slider;

        public void SetMinMax(float min, float max)
        {
            _slider.minValue = min;
            _slider.maxValue = max;
            TrySetText();
        }

        public void SetAction(Action<float> onValueChanged)
        {
            void Action(float value)
            {
                onValueChanged?.Invoke(value);

                var t = Mathf.InverseLerp(_slider.minValue, _slider.maxValue, value);
                t = Mathf.Lerp(_slider.minValue, _slider.maxValue, t);

                TrySetText();
            }

            _slider.onValueChanged.RemoveListener(Action);
            _slider.onValueChanged.AddListener(Action);
        }

        public void Set(float value)
        {
            _slider.value = value;
            TrySetText();
        }

        private void TrySetText()
        {
            if (_labelValue)
            {
                _labelValue.text = GetText();
            }
        }

        private string GetText() => string.Format(_format, _slider.value);
    }
}