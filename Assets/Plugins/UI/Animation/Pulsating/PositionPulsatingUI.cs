using System;
using UnityEngine;

namespace Plugins.Animated.Pulsating
{
	public class PositionPulsatingUI : PulsatingUI<Transform>
	{
		[SerializeField] private eLerpType lerpType = default;
		public Vector3 MinPosition = default;
		public Vector3 MaxPosition = default;

	
		public override void ValueApply()
		{
			switch (lerpType)
			{
				case eLerpType.Lerp:
					target.localPosition = Vector3.Lerp(MinPosition,MaxPosition,value);
					break;
				case eLerpType.LerpUnclamped:
					target.localPosition = Vector3.LerpUnclamped(MinPosition,MaxPosition,value);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			base.ValueApply();
		}

		private enum eLerpType
		{
			Lerp = 0,
			LerpUnclamped = 1,
		}
	}
}
