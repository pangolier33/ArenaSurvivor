using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;

namespace Bones.Gameplay.Stats.Processors
{
	internal sealed class ChainStatProcessor<T> : IStatProcessor<T>
	{
		private readonly LinkedList<IStatProcessor<T>> _processors = new();
		private readonly Subject<IStatProcessor<T>> _nodeAdded = new();
		
		T IStatProcessor<T>.Process(T from) => _processors.Aggregate(from, (value, processor) => processor.Process(value));
		public override string ToString() => $"ChainStatProcessor<{typeof(T)}>: [{string.Join(", ", Processors)}]";
		internal IObservable<IStatProcessor<T>> NodeAdded => _nodeAdded;
		internal IEnumerable<IStatProcessor<T>> Processors => _processors;
		internal LinkedListNode<IStatProcessor<T>> Add(IStatProcessor<T> item)
		{
			_nodeAdded.OnNext(item);
			return _processors.AddFirst(item);
		}
	}
}