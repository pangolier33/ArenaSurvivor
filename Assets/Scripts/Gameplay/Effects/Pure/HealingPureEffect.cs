using System;
using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Effects.Transitive.Depth;

namespace Bones.Gameplay.Effects.Pure
{
	[Serializable]
	public class HealingPureEffect : GetVariablePureEffect<float>
	{
		protected override void Invoke(ITrace trace, float value)
		{
			var stats = trace.Get<IPositionSource>().Stats;
			stats.Heal(value);
		}
	}
}