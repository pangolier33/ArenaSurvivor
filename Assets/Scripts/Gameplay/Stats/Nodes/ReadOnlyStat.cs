using System;
using Bones.Gameplay.Stats.Containers;
using Bones.Gameplay.Stats.Nodes;
using Bones.Gameplay.Stats.Processors;
using UniRx;

namespace Bones.Gameplay.Effects.Transitive.Depth
{
	public record ReadOnlyStat<T>(T BaseValue) : RecordStat, IGetStat<T>, IObservable<T>
	{
		private readonly ReactiveProperty<T> _value = new(BaseValue);
		private IStatProcessor<T> _processor = DummyStatProcessor<T>.Instance;

		public T BaseValue
		{
			get => _value.Value;
			set => _value.Value = value;
		}

		ref IStatProcessor<T> IGetStat<T>.Processor => ref _processor;
		public T Get() => _processor.Process(BaseValue);
		public IDisposable Subscribe(IObserver<T> observer) => _value.Subscribe(observer);
	}
}