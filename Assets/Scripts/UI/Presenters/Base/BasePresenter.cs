using System;
using Bones.UI.Bindings.Base;
using UniRx;
using UnityEngine;

namespace Bones.UI.Presenters
{
	public abstract class BasePresenter : MonoBehaviour, IDisposable
	{
		private readonly CompositeDisposable _subscriptions = new();
        
		public void Dispose()
		{
			_subscriptions.Dispose();
		}

		protected void Subscribe<T>(IObservable<T> model, IBinding binding)
		{
			_subscriptions.Add(model.Subscribe(Cast<T>(binding)));
		}

		protected void OnNext<T>(T value, IBinding binding)
		{
			Cast<T>(binding).OnNext(value);
		}

		private static IObserver<T> Cast<T>(IBinding binding)
		{
			if (binding is not IObserver<T> observer)
				throw new InvalidCastException($"{binding} is not observing {typeof(T)}");
			return observer;
		}

		private void OnDestroy()
		{
			Dispose();
		}
	}
}