using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Stats.Containers.Graphs;
using UnityEngine;

namespace Bones.Gameplay.Effects.Pure
{
	public abstract class BoostFromStatPEC<T> : BoostPEC<T>
	{
		[SerializeField] private StatName _name;
		protected sealed override T RetrieveAmplifier(ITrace trace) => trace.GetStatValue<T>(_name);
	}
}