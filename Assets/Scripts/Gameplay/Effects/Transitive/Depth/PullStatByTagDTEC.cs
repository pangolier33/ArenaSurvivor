using System;
using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Effects.Transitive.Depth;
using Bones.Gameplay.Stats.Containers;
using Bones.Gameplay.Stats.Containers.Graphs;

namespace Bones.Gameplay.Effects.Pure
{
	[Serializable]
	public sealed class PullStatByTagDTEC : AddTEC<IStat>
	{
		protected override IStat RetrieveArg(ITrace trace) => trace.GetStat(trace.GetStatValue<StatName>(StatName.NameTag));
	}
}