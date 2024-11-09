using System;
using Bones.Gameplay.Stats.Units;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Bones.Gameplay
{
	[Serializable]
	public sealed class PointsBuilder : UncertainStatBuilderBase<Points, Amount, Amount>
	{
		[SuffixLabel("$CalculatedChance")]
		[SerializeField]
		private int _base;
		[SerializeField,LabelText("K (Coefficient)")]
		private double _k;

		public override string ToString() => _base.ToString();
		protected override Points GetValue() => new(_base, _k);
		
#if UNITY_EDITOR
		public float CalculatedChance => (float)(1d - Math.Exp(-_base * _k));
#endif
	}
}