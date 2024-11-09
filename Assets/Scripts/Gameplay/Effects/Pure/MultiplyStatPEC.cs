using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Effects.Pure;
using Bones.Gameplay.Stats.Containers.Graphs;
using Bones.Gameplay.Stats.Units.Operations;
using UnityEngine;

namespace Bones.Gameplay.Effects.Transitive.Depth
{
	public abstract class MultiplyStatPEC<TFrom, TMultiplier> : PureEffect where TFrom : IMultiplySupportable<TMultiplier, TFrom>
	{
		[SerializeField] private StatName _sourceName;
		[SerializeField] private StatName _operandName;
        
		protected sealed override void Invoke(ITrace trace)
		{
			trace.MultiplyStatValue<TFrom, TMultiplier>(_sourceName, trace.GetStatValue<TMultiplier>(_operandName));
		}
	}
}