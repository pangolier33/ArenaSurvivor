using System;
using Bones.Gameplay.Effects.Containers.Projectiles;
using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Stats.Units;
using UnityEngine;

namespace Bones.Gameplay.Effects.Pure
{
	[Serializable]
	public class MoveHandleForwardPEC : GetStatPureEffect<Value>
	{
		protected override void Invoke(ITrace trace, Value speed)
		{
			trace.Get<IProjectileHandle>().Instance.transform.Translate(speed * Vector2.up, Space.Self);
		}
	}
}