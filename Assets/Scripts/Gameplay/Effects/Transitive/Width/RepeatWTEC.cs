using System;
using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Stats.Containers.Graphs;
using Bones.Gameplay.Stats.Units;
using UnityEngine;

namespace Bones.Gameplay.Effects.Transitive.Width
{
	[Serializable]
	public class RepeatWTEC : TransitiveEffect
	{
		[SerializeField] private StatName _countName;
		protected override void Invoke(IMutableTrace trace, IEffect next)
		{
			var count = trace.GetStatValue<Amount>(_countName);
			for (var i = 0; i < count; i++)
				next.Invoke(trace);
		}
	}
}