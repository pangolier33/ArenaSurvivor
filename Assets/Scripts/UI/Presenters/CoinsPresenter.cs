using System;
using Bones.Gameplay.Meta.Currencies;
using Zenject;

namespace Bones.UI.Presenters
{
    public class CoinsPresenter : InjectedPresenter<float>
    {
		private ICurrencyStorage _currencyStorage;

		protected override IObservable<float> RetrieveModel() => _currencyStorage.LevelCurrency.ObservableValue;

        [Inject]
        private void Inject(ICurrencyStorage currencyStorage)
        {
			_currencyStorage = currencyStorage;
        }
    }
}