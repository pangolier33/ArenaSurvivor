using UnityEngine.UI.ProceduralImage;

namespace Plugins.mitaywalle.ACV.Runtime
{
	public class ACV_ProceduralImage : ACV_Component<ProceduralImage>
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