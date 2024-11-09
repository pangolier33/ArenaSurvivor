using System;
using Bones.Gameplay.Effects.Containers.Projectiles;
using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Effects.Transitive.Depth;
using Bones.Gameplay.Stats.Containers.Graphs;
using Bones.Gameplay.Stats.Units;
using UnityEngine;

namespace Bones.Gameplay.Effects.Pure
{
	[Serializable]
	public class MoveHandleToPEC : IEffect
	{
		[SerializeField] private StatName _targetName;
		[SerializeField] private StatName _speedName;
		[SerializeReference] private IEffect _reachedEffect;

		public void Invoke(IMutableTrace trace)
		{
			var target = (Vector3)trace.GetStatValue<Pair>(_targetName);
			var transform = trace.Get<IProjectileHandle>().Instance.transform;
			
			if (Math.Abs(transform.position.x - target.x) < 0.1f && Math.Abs(transform.position.y - target.y) < 0.1f)
			{
				_reachedEffect.Invoke(trace);
				return;
			}
			
			var delta = target - transform.position;
			var speed = (float)trace.GetStatValue<Value>(_speedName);
			transform.Translate(delta.normalized * speed);
		}
	}
}