using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Stats.Containers;
using Bones.Gameplay.Stats.Containers.Graphs;
using UnityEngine;

namespace Bones.Gameplay.Effects.Transitive.Depth
{
	public abstract class InjectStatDTEC : AddTEC<IStatMap>
	{
		[SerializeField] private StatName _name;
		
		protected sealed override IStatMap RetrieveArg(ITrace trace)
		{
			return new UnitStatMap(_name, RetrieveStat(trace));
		}

		protected abstract IStat RetrieveStat(ITrace trace);
	}
}