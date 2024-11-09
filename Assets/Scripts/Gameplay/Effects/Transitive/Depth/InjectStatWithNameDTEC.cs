using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Stats.Containers;
using Bones.Gameplay.Stats.Containers.Graphs;
using UnityEngine;

namespace Bones.Gameplay.Effects.Transitive.Depth
{
	public abstract class InjectStatWithNameDTEC : AddTEC<IStatMap>
	{
		[SerializeField] private StatName _newName;
		[SerializeField] private StatName _sourceName;
		
		protected sealed override IStatMap RetrieveArg(ITrace trace) => new UnitStatMap(_newName, RetrieveStat(trace, _sourceName));
		protected abstract IStat RetrieveStat(ITrace trace, StatName name);
	}
}