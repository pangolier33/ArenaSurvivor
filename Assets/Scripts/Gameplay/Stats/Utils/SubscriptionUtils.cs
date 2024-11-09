using System;
using System.Collections.Generic;

namespace Bones.Gameplay.Stats.Utils
{
	internal static class SubscriptionUtils
	{
		public static IDisposable CreateSubscription<T>(this LinkedListNode<T> node)
		{
			return new DelegateDisposable(() => node.List?.Remove(node));
		}
		
		private class DelegateDisposable : IDisposable
		{
			private Action _delegate;

			public DelegateDisposable(Action @delegate)
			{
				_delegate = @delegate;
			}

			public void Dispose()
			{
				if (_delegate == null)
					return;
				_delegate();
				_delegate = null;
			}
		}
	}
}