namespace Bones.Gameplay.Meta.Currencies
{
	public interface ICurrencySave
	{
		CurrencyType Type { get; }
		float Value { get; }
	}
}
