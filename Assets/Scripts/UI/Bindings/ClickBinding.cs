using System;
using Bones.UI.Bindings.Base;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI.Bindings
{
	[Serializable]
	public sealed class ClickBinding : BaseBinding<Action>
	{
		[SerializeField] private Button _button;
		private UnityAction _callbackWrapper;
		private UnityAction _callback;
		
		public override void OnNext(Action value)
		{
			if (_callbackWrapper == null)
				_button.onClick.AddListener(_callbackWrapper = OnClicked);
			_callback = new UnityAction(value);
		}

		public override void OnCompleted()
		{
			if (_callbackWrapper != null)
				_button.onClick.RemoveListener(_callbackWrapper);
		}

		private void OnClicked() => _callback.Invoke();
	}
}