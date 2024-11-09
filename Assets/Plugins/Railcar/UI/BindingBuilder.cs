using System;

namespace Railcar.UI
{
	public abstract class BindingBuilder
	{
		internal abstract IObserver<T> Build<T>();
	}
}