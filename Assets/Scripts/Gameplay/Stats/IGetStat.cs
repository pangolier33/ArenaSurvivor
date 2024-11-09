using Bones.Gameplay.Stats.Processors;

namespace Bones.Gameplay.Stats.Containers
{
	public interface IGetStat<T> : IStat
	{
		T Get();
		internal ref IStatProcessor<T> Processor { get; }
	}
}