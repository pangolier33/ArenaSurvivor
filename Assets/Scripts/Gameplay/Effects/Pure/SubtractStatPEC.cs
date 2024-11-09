using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Effects.Pure;
using Bones.Gameplay.Stats.Containers.Graphs;
using UnityEngine;

namespace Bones.Gameplay.Effects.Transitive.Depth
{
	public abstract class SubtractStatPEC<T> : PureEffect
	{
		[SerializeField] private StatName _sourceName;
		[SerializeField] private StatName _operandName;
        
		protected override void Invoke(ITrace trace)
		{
			trace.SubtractStatValue(_sourceName, trace.GetStatValue<T>(_operandName));
		}
	}
}