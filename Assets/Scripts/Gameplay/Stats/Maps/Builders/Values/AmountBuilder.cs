using System;
using Bones.Gameplay.Stats.Units;
using UnityEngine;

namespace Bones.Gameplay
{
	[Serializable]
	public sealed class AmountBuilder : UncertainStatBuilderBase<Amount, Amount, Amount>
	{
		[SerializeField] private int _base;
		public override string ToString() => _base.ToString();
		protected override Amount GetValue() => new(_base);
	}
}