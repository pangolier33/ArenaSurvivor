using System;
using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Stats.Containers.Graphs;
using UnityEngine;

namespace Bones.Gameplay.Effects.Transitive.Depth
{
	[Serializable]
	public class VectorMultiplyDTEC : SetVariableEffectWrapper<Vector2>
	{
		[SerializeField] private StatName _name;
		[SerializeField] private string _otherId;
		protected override Vector2 GetValue(ITrace provider)
		{
			return provider.GetVariable<Vector2>(_otherId) * provider.GetStatValue<Pair>(_name);
		}
	}
}