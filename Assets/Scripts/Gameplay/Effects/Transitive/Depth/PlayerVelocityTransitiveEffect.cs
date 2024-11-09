using System;
using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Map;
using UnityEngine;
using Zenject;

namespace Bones.Gameplay.Effects.Transitive.Depth
{
	[Serializable]
	public class PlayerVelocityTransitiveEffect : SetVariableEffectWrapper<Vector2>
	{
		private PositionTracker _tracker;

		protected override Vector2 GetValue(ITrace provider)
		{
			return _tracker.Velocity;
		}
		
		[Inject]
		private void Inject(PositionTracker tracker)
		{
			_tracker = tracker;
		}
	}
}