using Bones.Gameplay.Meta.Currencies;
using TMPro;
using UnityEngine;
using Zenject;
using UniRx;

namespace UI.Views
{
	public class CurrencyView : MonoBehaviour, IInitializable
	{
		[SerializeField] private TMP_Text label;
		[SerializeField] private CurrencyType _type;

		[Inject] private ICurrencyStorage _currencyStorage;

		void IInitializable.Initialize()
		{
			var currency = _currencyStorage.Get(_type);
			currency.ObservableValue.Subscribe(OnSilverChanged);
			OnSilverChanged(currency.Value);
		}

		private void OnSilverChanged(float value)
		{
			label.text = value.ToString();
		}
	}
}