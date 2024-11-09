using System;
using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Effects.Transitive.Depth;

namespace Bones.Gameplay.Effects.Pure
{
	[Serializable]
	public sealed class DamagingPureEffect : GetVariablePureEffect<float>
	{
		protected override void Invoke(ITrace trace, float value)
		{
			var enemy = trace.Get<IPositionSource>().Stats;
			enemy.TakeDamage(value);
		}
	}
}