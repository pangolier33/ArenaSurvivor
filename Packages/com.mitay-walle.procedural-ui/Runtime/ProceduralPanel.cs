using UnityEngine;
using UnityEngine.UI;

namespace Mitaywalle.ProceduralUI
{
    public class ProceduralPanel : MonoBehaviour
    {
        [SerializeField] private SwitchGo _switchTabs;
        [SerializeField] private ToggleGroup _toggleGroup;
        [SerializeField] private StackWidget _parentToggles;
        [SerializeField] private Transform _parentTabs;

        [SerializeField] private HeaderWidget Header;
        [SerializeField] private ButtonWidget _closeButton;

        [SerializeField] private StackWidget _tabPrefab;

        private void Start()
        {
            Init();
        }
        
        public void Init()
        {
            _toggleGroup.allowSwitchOff = false;
            if (_closeButton) _closeButton.SetAction(() => gameObject.SetActive(false));
        }

        public StackWidget AddTab(string tabHeader)
        {
            StackWidget tab = Instantiate(_tabPrefab, _parentTabs);
            tab.Widgets.SetStackPrefab(_tabPrefab);
            _switchTabs.Targets.Add(tab.gameObject);
            tab.name = $"Tab Stack {tabHeader}";

            int index = tab.transform.GetSiblingIndex();

            void OnValueChanged(bool value)
            {
                if (value)
                {
                    _switchTabs.ShowOnly(index);
                }
            }

            ToggleWidget toggle = _parentToggles.Widgets.CreateToggle(tabHeader, OnValueChanged);
            _toggleGroup.RegisterToggle(toggle.GetTarget());
            toggle.GetTarget().group = _toggleGroup;
            tab.Layout.MakeVertical();
            tab.StretchToParent();

            return tab;
        }


        public void Set(string header)
        {
            if (Header) Header.Set(header);
        }
    }
}
