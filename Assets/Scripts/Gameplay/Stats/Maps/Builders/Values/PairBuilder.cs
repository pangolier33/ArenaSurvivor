using System;
using Bones.Gameplay.Effects.Transitive.Depth;
using Bones.Gameplay.Stats.Units;
using UnityEngine;

namespace Bones.Gameplay
{
	[Serializable]
	public class PairBuilder : UncertainStatBuilderBase<Pair, Pair, Pair>
	{
		[SerializeField]
		private double _x;
		
		[SerializeField]
		private double _y;

		protected override Pair GetValue() => new(new Value(_x), new Value(_y));
	}
}