using System;
using Bones.Gameplay.Effects.Provider;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Bones.Gameplay.Effects.Transitive.Depth
{
	[Serializable]
	public sealed class RandomGateTransitiveEffect : TransitiveEffect
	{
		[SerializeField] private string _id;
		protected override void Invoke(IMutableTrace trace, IEffect next)
		{
			var probability = trace.GetVariable<float>(_id);
			if (Random.value < probability)
				next.Invoke(trace);
		}
	}
}