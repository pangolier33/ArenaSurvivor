using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Mitaywalle.ProceduralUI
{
    [Serializable]
    public class WidgetsFactory
    {
        [SerializeField] private Transform _parent;
        [SerializeField] private HeaderWidget _header;
        [SerializeField] private ButtonWidget _button;
        [SerializeField] private ToggleWidget _toggle;
        [SerializeField] private SliderWidget _slider;
        [SerializeField] private LabelWidget _label;
        [SerializeField] private InputFieldWidget _input;
        [SerializeField] private Widget _separator;
        [SerializeField] private StackWidget _stack;

        private Dictionary<string, Widget> _widgets = new();

        public void SetStackPrefab(StackWidget prefab) => _stack = prefab;
        public ToggleGroup CreateToggleGroup()
        {
            return _parent.gameObject.AddComponent<ToggleGroup>();
        }

        public Widget CreateSeparator(string key = null)
        {
            if (string.IsNullOrEmpty(key))
            {
                key = Guid.NewGuid().ToString();
            }

            return Create(key, _separator, null);
        }

        public SliderWidget CreateSlider(string key, float max = 1, float min = 0)
        {
            void Action(SliderWidget widget)
            {
                var slider = widget.GetTarget();
                slider.minValue = min;
                slider.maxValue = max;
            }

            return Create(key, _slider, Action);
        }

        public HeaderWidget CreateHeader(string key, string value = null,bool addLocale = true)
        {
            if (string.IsNullOrEmpty(value)) value = key;
            void Action(HeaderWidget widget) => widget.Set(value,addLocale);

            return Create(key, _header, Action);
        }

        public StackWidget CreateStack(string key)
        {
            void Action(StackWidget widget)
            {
                widget.Widgets._stack = _stack;
                widget.StretchToParent();
            }

            return Create(key, _stack, Action);
        }

        public ButtonWidget CreateButton(string key, Action onClick)
        {
            void Action(ButtonWidget widget)
            {
                widget.Set(key);
                widget.SetAction(onClick);
            }

            return Create(key, _button, Action);
        }


        public ToggleWidget CreateToggle(string key, Action<bool> onValueChanged)
        {
            void Action(ToggleWidget widget)
            {
                widget.Set(key);
                widget.SetAction(onValueChanged);
            }

            return Create(key, _toggle, Action);
        }

        // public LabelWidget CreateLabel(string key, string value = null)
        // {
        //     if (string.IsNullOrEmpty(value)) value = key;
        //     void Action(LabelWidget widget) => widget.Set(value);
        //
        //     return Create(key, _label, Action);
        // }
        //
        // public InputFieldWidget CreateTextInput(string key, string value = null)
        // {
        //     if (string.IsNullOrEmpty(value)) value = key;
        //     void Action(InputFieldWidget widget) => widget.Set(value);
        //
        //     return Create(key, _input, Action);
        // }

        private T Create<T>(string key, T prefab, Action<T> action)
            where T : Widget
        {
            if (!CheckCanAddWidget(key)) return null;

            var widget = Object.Instantiate(prefab, _parent);
            action?.Invoke(widget);

            widget.name = widget.name.Replace("(Clone)", " - ") + key;

            _widgets.Add(key, widget);

            return widget;
        }

        private bool CheckCanAddWidget(string key)
        {
            if (!_widgets.ContainsKey(key)) return true;

            Debug.LogError($"'{key}' already exist!", _parent);

            return false;
        }
    }
}
