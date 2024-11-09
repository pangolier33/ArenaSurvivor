using UnityEngine;

namespace Plugins.Animated.Pulsating
{
	public sealed class RotationPulsatingUI : PulsatingUI<Transform>
	{
		public Vector3 MinPosition;
		public Vector3 MaxPosition;
	
		public override void ValueApply()
		{
			target.localRotation = Quaternion.Euler(Vector3.Lerp(MinPosition,MaxPosition,value));
			base.ValueApply();
		}
	}
}
