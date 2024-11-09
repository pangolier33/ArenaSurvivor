using Bones.Gameplay.Stats.Processors;
using Bones.Gameplay.Stats.Units.Operations;

namespace Bones.Gameplay.Stats.Containers
{
	public record UncertainStat<TValue, TAdd, TSubtract>(TValue BaseValue) : Stat<TValue>(BaseValue), IAddStat<TAdd>, ISubtractStat<TSubtract>
		where TValue : IAddSupportable<TAdd, TValue>, ISubtractSupportable<TSubtract, TValue>
	{
		private IStatProcessor<TAdd> _addProcessor = DummyStatProcessor<TAdd>.Instance;
		private IStatProcessor<TSubtract> _subtractProcessor = DummyStatProcessor<TSubtract>.Instance;

		ref IStatProcessor<TAdd> IAddStat<TAdd>.Processor => ref _addProcessor;
		ref IStatProcessor<TSubtract> ISubtractStat<TSubtract>.Processor => ref _subtractProcessor;
		
		public void Add(TAdd value)
		{
			Set(BaseValue.Add(_addProcessor.Process(value)));
		}

		public void Subtract(TSubtract value)
		{
			Set(BaseValue.Subtract(_subtractProcessor.Process(value)));
		}
	}
}