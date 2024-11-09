using System;
using UnityEngine;

namespace Railcar.UI
{
	[Serializable]
	internal sealed class UnitBindingBuilder : BindingBuilder
	{
		[SerializeReference] private IBinding _binding = default!;

		internal override IObserver<T> Build<T>() => _binding is null ? DummyObserver<T>.Instance 
			: (IObserver<T>)_binding;

		private class DummyObserver<T> : IObserver<T>
		{
			private static IObserver<T> s_instance;
			private DummyObserver() {}
			
			public static IObserver<T> Instance => s_instance ??= new DummyObserver<T>();
			
			public void OnCompleted() { }
			public void OnError(Exception error) { }
			public void OnNext(T value) { }
		}
	}
}