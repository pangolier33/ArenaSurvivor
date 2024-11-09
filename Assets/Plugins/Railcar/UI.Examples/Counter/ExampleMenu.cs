using System;
using System.Collections.Generic;
using UnityEngine;

namespace Railcar.UI
{
	public sealed class ExampleMenu : BaseMenu
	{
		[SerializeReference] private BindingBuilder _propBinding;
		
		private IService _service;
		
		public void Inject(IService service)
		{
			_service = service;
		}

		protected override IEnumerable<IDisposable> SetupBindings()
		{
			yield return _propBinding.Bind(_service.Icon);
			yield return _propBinding.Bind(_service.Value);
			yield return _propBinding.Bind((Action)OnClicked);
		}
		
		private void OnClicked()
		{
			_service.Update();
		}
	}
}