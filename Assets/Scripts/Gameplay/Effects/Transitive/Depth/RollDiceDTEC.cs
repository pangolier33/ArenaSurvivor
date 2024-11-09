using System;
using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Stats.Containers.Graphs;
using Bones.Gameplay.Stats.Units;
using Railcar.Dice;
using UnityEngine;
using Zenject;

namespace Bones.Gameplay.Effects.Transitive.Depth
{
	[Serializable]
	public sealed class RollDiceDTEC : TransitiveEffect
	{
		[SerializeField] private StatName _chanceName;
		private IDice _dice;
		
		protected override void Invoke(IMutableTrace trace, IEffect next)
		{
			var chance = trace.GetStatValue<Value>(_chanceName);
			if (_dice.Roll(chance))
				next.Invoke(trace);
		}

		[Inject]
		private void Inject(IDice dice)
		{
			_dice = dice;
		}
	}
}