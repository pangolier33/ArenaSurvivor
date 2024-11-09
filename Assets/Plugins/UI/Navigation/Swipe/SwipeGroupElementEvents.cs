using UnityEngine;
using UnityEngine.Events;

namespace UI.Navigation.Swipe
{
	public class SwipeGroupElementEvents : MonoBehaviour, ISwipeGroupElement
	{
		public UnityEvent OnPostInit;
		public UnityEvent OnSelect;
		public UnityEvent OnDeselect;
		public UnityEvent OnDeselectEnd;
		public UnityEvent OnSelectSwitch;

		RectTransform ISwipeGroupElement.rectTransform => transform as RectTransform;

		void ISwipeGroupElement.PostInit()
		{
			OnPostInit?.Invoke();
		}

		void ISwipeGroupElement.OnSelected()
		{
			OnSelect?.Invoke();
		}

		void ISwipeGroupElement.OnDeselected()
		{
			OnDeselect?.Invoke();
		}

		void ISwipeGroupElement.OnDeselectEnd()
		{
			OnDeselectEnd?.Invoke();
		}

		void ISwipeGroupElement.OnSelectSwitch()
		{
			OnSelectSwitch?.Invoke();
		}
	}
}