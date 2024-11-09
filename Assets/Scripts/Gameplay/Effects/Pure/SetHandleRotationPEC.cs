using System;
using Bones.Gameplay.Effects.Containers.Projectiles;
using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Effects.Transitive.Depth;
using UnityEngine;

namespace Bones.Gameplay.Effects.Pure
{
	[Serializable]
	public sealed class SetHandleRotationPEC : GetStatPureEffect<Pair>
	{
		protected override void Invoke(ITrace trace, Pair value)
		{
			trace.Get<IProjectileHandle>().Instance.transform.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.up, value));
		}
	}
}