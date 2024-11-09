using Plugins.mitaywalle.ACV.Runtime;
namespace Samples.ACV___Animated_Change_Value._1._0._0.ACV_for_SlicedFilledImage.Scripts
{
	public class ACV_SlicedFilledImage : ACV_Component<SlicedFilledImage>
	{
		public override void SetValue()
		{
			_target.fillAmount = (_animatedValue - _minValue) * 1f / (_maxValue - _minValue);

			base.SetValue();
		}

		public override void DisableTarget()
		{
			_target.gameObject.SetActive(false);
		}
	}
}