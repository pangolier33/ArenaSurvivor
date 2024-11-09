using System;
using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Stats.Containers;
using Bones.Gameplay.Stats.Containers.Graphs;
using Bones.Gameplay.Stats.Units;
using UnityEngine;

namespace Bones.Gameplay.Effects.Transitive.Depth
{
	[Serializable]
	public class MultiplyValueByAmountDTEC : InjectStatWithNameDTEC
	{
		[SerializeField] private StatName _amountName; 
		protected override IStat RetrieveStat(ITrace trace, StatName name)
		{
			var sourceValue = trace.GetStatValue<Value>(name);
			var operandValue = trace.GetStatValue<Amount>(_amountName);
			return new ReadOnlyStat<Value>(new Value(sourceValue * operandValue));
		}
	}
}