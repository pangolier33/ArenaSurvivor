using System;
using Bones.Gameplay.Stats.Containers;
using Bones.Gameplay.Stats.Numbers;
using Bones.Gameplay.Stats.Units;
using UnityEngine;

namespace Bones.Gameplay
{
	[Serializable]
	public sealed class TimedValueBuilder : StatBuilderBase<TimedValue>
	{
		[SerializeField] private float _interval;
		[SerializeField] private float _amount;
		
		public override IStat BuildUncertain() => throw new NotSupportedException();

		protected override TimedValue GetValue()
		{
			return new TimedValue
			{
				Interval = new Value(_interval),
				Amount = new Value(_amount)
			};
		}
	}
}