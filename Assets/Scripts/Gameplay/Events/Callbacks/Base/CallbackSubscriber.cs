using Bones.Gameplay.Events.Args;
using System;
using UniRx;

namespace Bones.Gameplay.Events.Callbacks.Templates.Base
{
	public abstract class CallbackSubscriber<T>: IInjectable, ICallbackTemplate
		where T : IMessageArgs
	{
		public IDisposable Subscribe(IMessageReceiver broker)
		{
			var observable = broker.Receive<T>();
			var subscribe = observable.Subscribe<T>(GetCallback().OnNext);
			return subscribe;
		}

		protected abstract IObserver<T> GetCallback();
	}
}
