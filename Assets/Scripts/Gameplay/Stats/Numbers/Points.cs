using System;
using Bones.Gameplay.Stats.Units.Operations;
using UnityEngine;

namespace Bones.Gameplay.Stats.Units
{
	public readonly struct Points : IAddSupportable<Amount, Points>,
									ISubtractSupportable<Amount, Points>,
									IMultiplySupportable<Amount, Points>,
									
									IAddSupportable<Value, Value>,
									ISubtractSupportable<Value, Value>,
									IMultiplySupportable<Value, Value>,
									
									IMultiplySupportable<Points, Points>,
									IMultiplySupportable<Value, Points>,

									IComparable<Amount>,
									IComparable<Points>,
									IComparable<Value>
	{
		public Points(int value, double k)
		{
			Value = value;
			K = k;
		}

		public double Percent => CumulativeExp(Value, K);
		public int Value { get; }
		private double K { get; }

		// AMOUNTS
		public static Points operator +(Points left, Amount right) => new(left.Value + right, left.K);
		public Points Add(Amount right) => this + right;

		public static Points operator -(Points left, Amount right) => new(left.Value - right, left.K);
		public Points Subtract(Amount right) => this - right;

		public static Points operator *(Points left, Amount right) => new(left.Value * right, left.K);
		public Points Multiply(Amount right) => this * right;

		// VALUES
		public static Value operator +(Points left, Value right) => new(left.Percent + right);
		public Value Add(Value right) => this + right;
		
		public static Value operator -(Points left, Value right) => new(left.Percent - right);
		public Value Subtract(Value right) => this - right;
		
		public static Value operator *(Points left, Value right) => new(left.Percent * right);
		public Value Multiply(Value right) => this * right;
		
		// POINTS (SELF)
		public static Points operator *(Points left, Points right)
		{
			ThrowIfCoefficientDiffers(left, right);
			return new Points(left.Value * right.Value, left.K);
		}
		public Points Multiply(Points right) => this * right;
		
		public static bool operator >(Points left, Points right)
		{
			ThrowIfCoefficientDiffers(left, right);
			return left.Value > right.Value;
		}

		public static bool operator <(Points left, Points right)
		{
			ThrowIfCoefficientDiffers(left, right);
			return left.Value < right.Value;
		}

		public int CompareTo(Points other)
		{
			ThrowIfCoefficientDiffers(this, other);
			return Value.CompareTo(other.Value);
		}

		public int CompareTo(Amount other)
		{
			return Value.CompareTo(other.Value);
		}

		public int CompareTo(Value other)
		{
			return Percent.CompareTo(other.Base);
		}

		Points IMultiplySupportable<Value, Points>.Multiply(Value right)
		{
			return new(Mathf.FloorToInt(Value * right), K);
		}

		public override string ToString()
		{
			return $"{Value} pts ({Percent:P1})";
		}
		
		private static double CumulativeExp(int x, double k) =>
			x switch
			{
				> 0 => 1d - Math.Exp(-x * k),
				_ => 0d
			};

		private static void ThrowIfCoefficientDiffers(Points left, Points right)
		{
			// ReSharper disable once CompareOfFloatsByEqualityOperator
			if (left.K != right.K)
				throw new InvalidOperationException($"Can't compare two points structures with different coefficients: {left.K} and {right.K}");
		}
	}
}