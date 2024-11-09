using System;
using UniRx;

namespace Bones.Gameplay.Stats.Processors
{
	public class RxProcessor<T> : IStatProcessor<T>, IObservable<T>
	{
		private readonly Subject<T> _onProcessing = new();
		
		public T Process(T from)
		{
			_onProcessing.OnNext(from);
			return from;
		}
		
		public IDisposable Subscribe(IObserver<T> observer)
		{
			return _onProcessing.Subscribe(observer);
		}
	}
}