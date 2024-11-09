using System;
using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Events.Callbacks.Templates.Base;
using Bones.Gameplay.Meta.Currencies;
using Zenject;

namespace Bones.Gameplay.Effects.Pure
{
	[Serializable]
	public sealed class CoinPureEffect : GetVariablePureEffect<int>, IInjectable
	{
		private ICurrencyStorage _currencyStorage;

		protected override void Invoke(ITrace trace, int value)
		{
			_currencyStorage.LevelCurrency.Deposit(value);
		}

		[Inject]
		private void Inject(ICurrencyStorage currencyStorage)
		{
			_currencyStorage = currencyStorage;
		}
	}
}