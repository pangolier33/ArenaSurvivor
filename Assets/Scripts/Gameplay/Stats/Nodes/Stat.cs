using Bones.Gameplay.Effects.Transitive.Depth;
using Bones.Gameplay.Stats.Processors;

namespace Bones.Gameplay.Stats.Containers
{
	public record Stat<T>(T BaseValue) : ReadOnlyStat<T>(BaseValue), ISetStat<T>
	{
		private IStatProcessor<T> _setProcessor = DummyStatProcessor<T>.Instance;

		ref IStatProcessor<T> ISetStat<T>.Processor => ref _setProcessor;

		public void Set(T value)
		{
			BaseValue = _setProcessor.Process(value);
		}
	}
}