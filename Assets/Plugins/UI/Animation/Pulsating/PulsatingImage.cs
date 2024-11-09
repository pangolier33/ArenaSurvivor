using System;
using UnityEngine.UI;

namespace Plugins.Animated.Pulsating
{
	public class PulsatingImage : PulsatingUI<Image>
	{
		public enum FillType
		{
			Alpha = 0,
			Fill = 1,
		}

		public FillType type;
	
		public override void ValueApply()
		{
			switch (type)
			{
				case FillType.Alpha:
					var color = target.color;
					color.a = value;
					target.color = color;
					break;
				case FillType.Fill:
					target.fillAmount = value;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			base.ValueApply();
		}
	}
}
