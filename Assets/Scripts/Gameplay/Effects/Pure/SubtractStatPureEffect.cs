using System;
using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Effects.Pure;
using Bones.Gameplay.Stats.Containers.Graphs;
using UnityEngine;

namespace Bones.Gameplay.Effects.Transitive.Depth
{
	[Serializable]
	public sealed class SubtractStatPureEffect : GetVariablePureEffect<float>
	{
		[SerializeField] private StatName _name;
		
		protected override void Invoke(ITrace trace, float value)
		{
			var stats = trace.Get<IPositionSource>().Stats;
			stats.Subtract(_name, value);
		}
	}
}