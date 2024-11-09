using System;
using UniRx;

namespace Bones.Gameplay.Stats.Processors
{
	internal class ObservableStatProcessor<T> : IStatProcessor<T>, IObservable<T>
	{
		private readonly Subject<T> _subject = new();

		public ObservableStatProcessor(IStatProcessor<T> wrappedProcessor)
		{
			WrappedProcessor = wrappedProcessor;
		}

		internal IStatProcessor<T> WrappedProcessor { get; }

		public T Process(T from)
		{
			var result = WrappedProcessor.Process(from);
			_subject.OnNext(result);
			return result;
		}

		public IDisposable Subscribe(IObserver<T> observer)
		{
			return _subject.Subscribe(observer);
		}
	}
}