using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Effects.Pure;
using Bones.Gameplay.Stats.Containers.Graphs;
using UnityEngine;

namespace Bones.Gameplay.Effects.Transitive.Depth
{
	public class SetStatPEC<T> : PureEffect
	{
		[SerializeField] private StatName _leftName;
		[SerializeField] private StatName _rightName;
        
		protected override void Invoke(ITrace trace)
		{
			trace.SetStatValue(_leftName, trace.GetStatValue<T>(_rightName));
		}
	}
}