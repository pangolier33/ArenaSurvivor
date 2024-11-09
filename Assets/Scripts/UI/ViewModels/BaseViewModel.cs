using System;
using Bones.Gameplay.Startup;
using UniRx;

namespace Bones.UI.Presenters.New
{
	public abstract class BaseViewModel<T> : BaseBinder, IViewModel<T>
	{
		private IDisposable _subscription;

		public bool IsBound => _subscription != null;
        
		public void Bind(T model)
		{
			_subscription?.Dispose();
			_subscription = SetupBindings(model);
		}
        
		protected abstract IDisposable SetupBindings(T collection);

		private void OnDestroy()
		{
			_subscription.Dispose();
		}
	}
}