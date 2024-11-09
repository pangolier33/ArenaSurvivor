using System;
using System.Collections;
using System.Collections.Generic;
using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Stats.Containers.Graphs;
using Bones.Gameplay.Stats.Units;
using UnityEngine;

namespace Bones.Gameplay.Effects.Transitive.Width
{
	[Serializable]
	public sealed class DistributeWTEC : TransitiveEffect
	{
		[SerializeField] private StatName _countName;
		[SerializeField] private StatName _repeatsLimitName;
		
		protected override void Invoke(IMutableTrace trace, IEffect next)
		{
			var generalCount = (int)trace.GetStatValue<Amount>(_countName);
			var limitCount = (int)trace.GetStatValue<Amount>(_repeatsLimitName);
			
			var enumerator = trace.Get<IEnumerator>();
			var count = 0;

			var buffer = new List<object>();
			for (; count < generalCount && enumerator.MoveNext(); count++)
				buffer.Add(enumerator.Current);

			if (count == 0)
				return;
						
			var repeats = Math.Min(generalCount / count, limitCount);
			for (var i = 0; i < repeats; i++)
			for (var j = 0; j < count; j++)
				next.Invoke(trace.Add(buffer[j]));
		}
	}
}