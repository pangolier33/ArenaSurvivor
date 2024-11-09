using Bones.Gameplay.Meta.Currencies;

namespace Assets.Tests
{
	public class TestCurrencySave : ICurrencySave
	{
		public CurrencyType Type { get; init; }

		public float Value { get; init; }
	}
}
