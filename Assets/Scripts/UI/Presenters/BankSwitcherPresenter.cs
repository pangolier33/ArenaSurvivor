using Bones.Gameplay.Experience;
using System;
using UnityEngine;
using Zenject;

namespace Bones.UI.Presenters
{
    public class BankSwitcherPresenter : MonoBehaviour
    {
        private BankSwitcher _bankSwitcher;
		public event Action Switched;

		public BankType LastBankType => _bankSwitcher.LastBankType;

		[Inject]
        private void Inject(BankSwitcher bankSwitcher)
        {
            _bankSwitcher = bankSwitcher;
        }

        public void Switch(BankType bankType)
        {
            _bankSwitcher.Switch(bankType);
			Switched?.Invoke();
        }
    }
}