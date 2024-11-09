using System.Collections.Generic;
using Bones.Gameplay.Stats.Containers;
using Bones.Gameplay.Stats.Containers.Graphs;

namespace Bones.Gameplay.Effects.Provider
{
	public interface IStatMap
	{
		IEnumerable<KeyValuePair<StatName, IStat>> GetAll();
		bool TryGet(StatName name, out IStat stat);
		IStat Get(StatName name);
		bool Contains(StatName name);
	}
}