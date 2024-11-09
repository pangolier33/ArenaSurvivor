using System;
using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Players;
using Bones.Gameplay.Stats.Containers;
using Bones.Gameplay.Stats.Nodes;
using UnityEngine;

namespace Bones.Gameplay.Effects.Transitive.Depth
{
	[Serializable]
	public class CachePositionDTEC : InjectStatDTEC
	{
		[SerializeField] private bool _isDynamic = true;

		protected override IStat RetrieveStat(ITrace trace) => _isDynamic switch
		{
			false => new ReadOnlyStat<Pair>(trace.Get<ISpellActor>().Position),
			true => new DelegateStat<Pair>(() => trace.Get<ISpellActor>().Position)
		};
	}
}