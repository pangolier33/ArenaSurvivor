using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Railcar.UI.Bindings
{
	[Serializable]
	internal sealed class ButtonBinding : BaseBinding<Action>
	{
		[SerializeField] private Button _image;

		private Action _callback;
		private UnityAction _callbackWrapper;

		public override void OnNext(Action value)
		{
			if (_callbackWrapper == null)
				_image.onClick.AddListener(_callbackWrapper = OnClicked);
			_callback = value;
		}

		private void OnClicked()
		{
			_callback.Invoke();
		}
	}
}