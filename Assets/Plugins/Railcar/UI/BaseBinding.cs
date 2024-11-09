using System;

namespace Railcar.UI
{
	public abstract class BaseBinding<T> : IBinding, IObserver<T>
	{
		public void OnCompleted()
		{
			
		}

		public void OnError(Exception error)
		{
			throw error;
		}

		public abstract void OnNext(T value);
	}
}