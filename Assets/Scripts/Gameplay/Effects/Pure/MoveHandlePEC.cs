using System;
using Bones.Gameplay.Effects.Containers.Projectiles;
using Bones.Gameplay.Effects.Provider;
using UnityEngine;

namespace Bones.Gameplay.Effects.Pure
{
	[Serializable]
	public sealed class MoveHandlePEC : GetStatPureEffect<Vector2>
	{
		protected override void Invoke(ITrace trace, Vector2 value)
		{
			trace.Get<IProjectileHandle>().Instance.transform.Translate(
				value, 
				Space.World
			);
		}
	}
}