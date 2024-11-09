using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Railcar.UI
{
	public abstract class BaseMenu : MonoBehaviour, IMenu
	{
		private CompositeDisposable _subscriptions;

		public void Open() => _subscriptions = new(SetupBindings());
		public void Close() => _subscriptions?.Clear();

		protected abstract IEnumerable<IDisposable> SetupBindings();
	}
}