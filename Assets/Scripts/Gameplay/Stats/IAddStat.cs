using Bones.Gameplay.Stats.Processors;

namespace Bones.Gameplay.Stats.Containers
{
	public interface IAddStat<T> : IStat
	{
		void Add(T value);
		ref IStatProcessor<T> Processor { get; }
	}
}