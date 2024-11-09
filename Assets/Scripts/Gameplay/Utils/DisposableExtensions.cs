using System;
using System.Collections.Generic;
using UniRx;

namespace Bones.UI.Presenters.New
{
	public static class DisposableExtensions
	{
		public static IDisposable Concat(this IDisposable disposable, IDisposable other)
		{
			if (disposable is not ICollection<IDisposable> collection)
				return new CompositeDisposable(disposable, other);
            
			collection.Add(other);
			return disposable;
		}
	}
}