using Bones.Gameplay.Experience;
using UnityEngine;
using UnityEngine.UI;

namespace Bones.UI.Presenters
{
    public class BankSwitcherView : MonoBehaviour
    {
        [SerializeField] private BankSwitcherPresenter _bankSwitcher;
        [SerializeField] private Button _button;
        [SerializeField] private BankType _bankType;
		[SerializeField] private Image _icon;
		[SerializeField] private Sprite _onIcon;
		[SerializeField] private Sprite _offIcon;

        private void OnEnable()
        {
            _button.onClick.AddListener(OnClicked);
			UpdateSprite();
			_bankSwitcher.Switched += UpdateSprite;
        }

		private void OnDisable()
        {
            _button.onClick.RemoveListener(OnClicked);
			_bankSwitcher.Switched -= UpdateSprite;
		}

        private void OnClicked()
        {
            _bankSwitcher.Switch(_bankType);
        }

		private void UpdateSprite()
		{
			if (_bankSwitcher.LastBankType == _bankType)
				_icon.sprite = _onIcon;
			else
				_icon.sprite = _offIcon;
		}
	}
}