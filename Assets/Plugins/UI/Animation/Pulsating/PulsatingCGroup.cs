using UnityEngine;

#if UNITY_EDITOR

#endif

namespace Plugins.Animated.Pulsating
{
	public class PulsatingCGroup : PulsatingUI<CanvasGroup>
	{
		public override void ValueApply()
		{
			base.ValueApply();
			target.alpha = value;
		}
	}
}
