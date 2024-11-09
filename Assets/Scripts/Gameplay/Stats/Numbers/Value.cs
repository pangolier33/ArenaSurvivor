using System;
using Bones.Gameplay.Stats.Units.Operations;

namespace Bones.Gameplay.Stats.Units
{
	public readonly struct Value : IAddSupportable<Value, Value>,
								   ISubtractSupportable<Value, Value>,
								   IMultiplySupportable<Value, Value>,
								   IComparable<Value>
	{
		private static Value s_zero = new(0);
		private static Value s_half = new(.5d);
		private static Value s_unit = new(1);
		
		public Value(double @base) => Base = @base;

		public static implicit operator float(Value value) => (float)value.Base; 
		public static implicit operator double(Value value) => value.Base; 
		
		public static ref Value Zero => ref s_zero;
		public static ref Value Half => ref s_half;
		public static ref Value Unit => ref s_unit;
		public double Base { get; }
		
		public static Value operator +(Value left, Value right) => new(left.Base + right.Base);
		public Value Add(Value right) => this + right;
		
		public static Value operator -(Value left, Value right) => new(left.Base - right.Base);
		public Value Subtract(Value right) => this - right;
		
		public static Value operator *(Value left, Value right) => new(left.Base * right.Base);
		public Value Multiply(Value right) => this * right;
		
		public static bool operator >(Value left, Value right) => left.Base > right.Base;
		public static bool operator <(Value left, Value right) => left.Base < right.Base;
		public static bool operator >=(Value left, Value right) => left.Base >= right.Base;
		public static bool operator <=(Value left, Value right) => left.Base <= right.Base;
		public static bool operator ==(Value left, Value right) => left.Base == right.Base;
		public static bool operator !=(Value left, Value right) => left.Base != right.Base;

		public int CompareTo(Value other) => Base.CompareTo(other.Base);

		public override string ToString()
		{
			return $"V({Base:F3})";
		}
	}
}