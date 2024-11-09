using Bones.Gameplay.Stats.Processors;

namespace Bones.Gameplay.Stats.Containers
{
	public interface ISetStat<T> : IStat
	{
		void Set(T value);
		ref IStatProcessor<T> Processor { get; }
	}
}