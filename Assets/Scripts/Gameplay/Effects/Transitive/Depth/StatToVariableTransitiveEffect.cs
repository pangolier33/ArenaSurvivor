using System;
using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Stats.Containers.Graphs;
using UnityEngine;

namespace Bones.Gameplay.Effects.Transitive.Depth
{
	[Serializable]
	public sealed class StatToVariableTransitiveEffect : SetVariableEffectWrapper<float>
	{
		[SerializeField] private StatName _name;

		protected override float GetValue(ITrace provider)
		{
			var entity = provider.Get<IPositionSource>();
			return entity.Stats.Get(_name);
		}
	}
}