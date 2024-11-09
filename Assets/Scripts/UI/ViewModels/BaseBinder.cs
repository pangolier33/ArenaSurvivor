using System;
using Bones.UI.Bindings.Base;
using UniRx.InternalUtil;
using UnityEngine;

namespace Bones.UI.Presenters.New
{
	public abstract class BaseBinder : MonoBehaviour
	{
		protected static IDisposable Bind<TValue>(IObservable<TValue> model, IBinding binding) => model.Subscribe(Cast<TValue>(binding));
		protected static void OnNext<TValue>(TValue value, IBinding binding) => Cast<TValue>(binding).OnNext(value);
		
		private static IObserver<TValue> Cast<TValue>(IBinding binding)
		{
			if (binding is null)
				return EmptyObserver<TValue>.Instance;								
			if (binding is not IObserver<TValue> observer)
				throw new InvalidCastException($"{binding} is not observing {typeof(TValue)}");
			return observer;
		}
	}
}