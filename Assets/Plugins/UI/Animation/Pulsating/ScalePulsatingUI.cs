using UnityEngine;

#if UNITY_EDITOR

#endif

namespace Plugins.Animated.Pulsating
{
	public sealed class ScalePulsatingUI : PulsatingUI<Transform>
	{
		public override void ValueApply()
		{
			target.localScale = new Vector3(value,value,value);
			base.ValueApply();
		}
	}
}
