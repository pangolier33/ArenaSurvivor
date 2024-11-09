using System;
using Bones.Gameplay.Stats.Units;
using Bones.Gameplay.Stats.Units.Operations;
using UnityEngine;

namespace Bones.Gameplay.Effects.Transitive.Depth
{
	public readonly struct Pair : IAddSupportable<Pair, Pair>,
								  ISubtractSupportable<Pair, Pair>,
								  IMultiplySupportable<Value, Pair>,
								  IMultiplySupportable<Pair, Pair>
	{
		private readonly Value _x;
		private readonly Value _y;

		public Pair(Value x, Value y)
		{
			_x = x;
			_y = y;
		}

		public static implicit operator Vector2(Pair pair) => new(pair._x, pair._y);
		public static implicit operator Vector3(Pair pair) => new(pair._x, pair._y);
		public static implicit operator Pair(Vector2 vector) => new(new Value(vector.x), new Value(vector.y));
		public static implicit operator Pair(Vector3 vector) => new(new Value(vector.x), new Value(vector.y));
		
		public Value Magnitude => new(Math.Sqrt(_x * _x + _y * _y));
		public Value SqrMagnitude => new(_x * _x + _y * _y); 
		
		public static Pair operator +(Pair left, Pair right) => new(left._x - right._x, left._y - right._y);
		public Pair Add(Pair right) => this + right;

		public static Pair operator -(Pair left, Pair right) => new(left._x - right._x, left._y - right._y);
		public Pair Subtract(Pair right) => this - right;

		public static Pair operator *(Pair left, Value right) => new(left._x * right, left._y * right);
		public Pair Multiply(Value right) => this * right;

		public static Pair operator *(Vector2 left, Pair right) => new(new Value(left.x) * right._x, new Value(left.y) * right._y);
		public static Pair operator *(Pair left, Pair right) => new(left._x * right._x, left._y * right._y);
		public Pair Multiply(Pair right) => this * right;

		public override string ToString()
		{
			return $"P2({_x}, {_y})";
		}
	}
}