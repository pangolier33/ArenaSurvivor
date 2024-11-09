using UnityEngine.UI;

namespace Plugins.Animated.Pulsating
{
	public class PulsatingGraphic : PulsatingUI<Graphic>
	{
		public override void ValueApply()
		{
			var color = target.color;
			color.a = value;
			target.color = color;
			base.ValueApply();
		}
	}
}
