using System;
using UniRx;

namespace Railcar.UI
{
	public static class BindingExtensions
	{
		public static IDisposable Bind<T>(this BindingBuilder builder, T value)
		{
			builder.Build<T>().OnNext(value);
			return Disposable.Empty;
		}

		public static IDisposable Bind<T>(this BindingBuilder builder, IObservable<T> model)
		{
			return model.Subscribe(builder.Build<T>());
		}
	}
}