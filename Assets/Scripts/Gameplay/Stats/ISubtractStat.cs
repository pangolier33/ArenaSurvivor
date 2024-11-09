using Bones.Gameplay.Stats.Processors;

namespace Bones.Gameplay.Stats.Containers
{
	public interface ISubtractStat<T> : IStat
	{
		void Subtract(T value);
		ref IStatProcessor<T> Processor { get; }
	}
}