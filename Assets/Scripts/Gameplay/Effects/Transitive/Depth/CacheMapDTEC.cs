using System;
using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Players;

namespace Bones.Gameplay.Effects.Transitive.Depth
{
	[Serializable]
	public class CacheMapDTEC : AddTEC<IStatMap>
	{
		protected override IStatMap RetrieveArg(ITrace trace)
		{
			return trace.Get<ISpellActor>().Stats;
		}
	}
}