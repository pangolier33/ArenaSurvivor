using System;
using System.Globalization;
using Bones.Gameplay.Stats.Units;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Bones.Gameplay
{
	[Serializable]
	public sealed class ValueBuilder : UncertainStatBuilderBase<Value, Value, Value>, IFormattable
	{
		[SerializeField, LabelText(" ")] private float _base;
		public override string ToString() => _base.ToString(CultureInfo.DefaultThreadCurrentCulture);
		public string ToString(string format, IFormatProvider formatProvider) => _base.ToString(format, formatProvider);

		protected override Value GetValue() => new(_base);
	}
}