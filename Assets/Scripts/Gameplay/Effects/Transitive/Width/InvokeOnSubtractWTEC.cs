using System;
using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Stats.Containers.Graphs;
using Bones.Gameplay.Stats.Utils;
using UnityEngine;

namespace Bones.Gameplay.Effects.Transitive.Width
{
	[Serializable]
	public class InvokeOnSubtractWTEC : TransitiveEffect
	{
		[SerializeField] private StatName _statName;
		protected override void Invoke(IMutableTrace trace, IEffect next)
		{
			var stat = trace.GetStat(_statName);
			var subscription = stat.SubscribeOnSubtract(() => next.Invoke(trace));
			trace.ConnectToClosest(subscription);
		}
	}
}