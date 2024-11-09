using System;
using UniRx;

namespace Bones.Gameplay.Meta.Currencies
{
	public class Currency
	{
		private readonly ReactiveProperty<float> _value = new();

		public float Value => _value.Value;
		public IObservable<float> ObservableValue => _value;
		public CurrencyType Type { get; private set; }

		public Currency(float initialValue, CurrencyType type) 
		{
			if (initialValue < 0)
				throw new ArgumentOutOfRangeException(nameof(initialValue));
			if (type == CurrencyType.None)
				throw new ArgumentException("Incorrectly assigned currency type");
			_value.Value = initialValue;
			Type = type;
		}

		public void Deposit(float value)
		{
			if (value < 0)
				throw new ArgumentOutOfRangeException(nameof(value));
			_value.Value += value;
		}

		public void Withdrow(float value)
		{
			if (value < 0)
				throw new ArgumentOutOfRangeException(nameof(value));
			if (value > Value)
				throw new InvalidOperationException($"Not enough balance. Requested: {value}, Available: {Value}");
			_value.Value -= value;
		}

		public void Reset()
		{
			_value.Value = 0;
		}
	}
}
