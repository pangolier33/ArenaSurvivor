using UnityEngine.UI;

namespace Plugins.mitaywalle.ACV.Runtime
{
	public class ACV_Slider : ACV_Component<Slider>
	{
		public override void DisableTarget()
		{
			_target.gameObject.SetActive(false);
		}
		public override void SetValue()
		{
			_target.maxValue = _maxValue;
			_target.value = _animatedValue;
			base.SetValue();
		}
	}
}