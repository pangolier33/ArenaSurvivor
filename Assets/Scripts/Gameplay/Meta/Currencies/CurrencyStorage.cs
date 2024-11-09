using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Bones.Gameplay.Meta.Currencies
{
	public class CurrencyStorage : ICurrencyStorage
	{
		private readonly List<Currency> _currencies = new()
			{
				new Currency(0, CurrencyType.Silver),
				new Currency(0, CurrencyType.Crystal),
				new Currency(0, CurrencyType.ArmorRecipe),
				new Currency(0, CurrencyType.BeltRecipe),
				new Currency(0, CurrencyType.BootsRecipe),
				new Currency(0, CurrencyType.GloveRecipe)
			};
		public Currency LevelCurrency { get; }

		public CurrencyStorage()
		{
			LevelCurrency = new Currency(0, CurrencyType.Silver);
		}

		public void ApplySave(IEnumerable<ICurrencySave> save)
		{
			foreach (var currencySave in save)
			{
				Currency currency = Get(currencySave.Type);
				currency.Reset();
				currency.Deposit(currencySave.Value);
			}
		}

		public Currency Get(CurrencyType type)
		{
			Currency currency = _currencies.FirstOrDefault(x => x.Type == type);
			if (currency == null)
				throw new InvalidOperationException($"Can't find currency {type}");

			return currency;
		}

		public void TransferLevelCurrencyToBalance()
		{
			var silver = _currencies.First(x => x.Type == CurrencyType.Silver);
			silver.Deposit(LevelCurrency.Value);
			LevelCurrency.Reset();
		}

		public IEnumerator<Currency> GetEnumerator()
		{
			return _currencies.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
