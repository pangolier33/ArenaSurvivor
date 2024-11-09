using Bones.Gameplay.Stats.Containers.Graphs;

namespace Bones.Gameplay.Spells.Classes
{
	public interface IStatInfo
	{
		StatName Name { get; }
		IStatBuilder Builder { get; }
	}
}