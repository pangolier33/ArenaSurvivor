using System;
using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Stats.Containers;
using Bones.Gameplay.Stats.Containers.Graphs;

namespace Bones.Gameplay.Effects.Transitive.Depth
{
	[Serializable]
	public sealed class CacheStatDTEC : InjectStatWithNameDTEC
	{
		protected override IStat RetrieveStat(ITrace trace, StatName name) => trace.GetStat(name);
	}
}