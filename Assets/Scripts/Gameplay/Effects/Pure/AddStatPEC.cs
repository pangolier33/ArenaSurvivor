using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Effects.Pure;
using Bones.Gameplay.Stats.Containers.Graphs;
using Bones.Gameplay.Stats.Units;
using UnityEngine;

namespace Bones.Gameplay.Effects.Transitive.Depth
{
	public abstract class AddStatPEC<T> : PureEffect
	{
		[SerializeField] private StatName _sourceName;
		[SerializeField] private StatName _operandName;
        
		protected sealed override void Invoke(ITrace trace)
		{
			trace.AddStatValue(_sourceName, trace.GetStatValue<T>(_operandName));
		}
	}
}