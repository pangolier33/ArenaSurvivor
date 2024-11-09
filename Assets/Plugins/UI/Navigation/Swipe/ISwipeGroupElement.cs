using UnityEngine;

namespace UI.Navigation.Swipe
{
	public interface ISwipeGroupElement
	{
		public void PostInit();
		public void OnSelected();
		public void OnDeselected();
		public void OnDeselectEnd();
		public void OnSelectSwitch();

		public RectTransform rectTransform { get; }
	}
}