using System.Collections.Generic;

namespace Bones.Gameplay.Meta.Currencies
{
	public interface ICurrencyStorage : IEnumerable<Currency>
	{
		Currency Get(CurrencyType type);
		Currency LevelCurrency { get; }
		void ApplySave(IEnumerable<ICurrencySave> save);
		void TransferLevelCurrencyToBalance();
	}
}