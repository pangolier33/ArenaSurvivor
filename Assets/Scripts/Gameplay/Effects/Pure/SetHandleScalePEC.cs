using System;
using Bones.Gameplay.Effects.Containers.Projectiles;
using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Effects.Transitive.Depth;

namespace Bones.Gameplay.Effects.Pure
{
	[Serializable]
	public sealed class SetHandleScalePEC : GetStatPureEffect<Pair>
	{
		protected override void Invoke(ITrace trace, Pair value)
		{
			trace.Get<IProjectileHandle>().Instance.transform.localScale = value;
		}
	}
}