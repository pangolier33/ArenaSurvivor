using System;
using Bones.Gameplay.Stats.Units.Operations;

namespace Bones.Gameplay.Stats.Units
{
	public readonly struct Amount : IAddSupportable<Amount, Amount>,
									ISubtractSupportable<Amount, Amount>,
									IMultiplySupportable<Amount, Amount>,
									IComparable<Amount>
	{
		public Amount(int value) => Value = value;

		public static implicit operator int(Amount amount) => amount.Value;

		internal int Value { get; }

		public static Amount operator +(Amount left, Amount right) => new(left.Value + right.Value);
		public Amount Add(Amount right) => this + right;
		
		public static Amount operator -(Amount left, Amount right) => new(left.Value - right.Value);
		public Amount Subtract(Amount right) => this - right;
		
		public static Amount operator *(Amount left, Amount right) => new(left.Value * right.Value);
		public Amount Multiply(Amount right) => this * right;


		public static Amount operator ++(Amount source) => new(source.Value + 1);
		public static bool operator >(Amount left, Amount right) => left.Value > right.Value;
		public static bool operator <(Amount left, Amount right) => left.Value < right.Value;

		public int CompareTo(Amount other) => Value.CompareTo(other.Value);
		
		public override string ToString() => $"{Value} times";
	}
}