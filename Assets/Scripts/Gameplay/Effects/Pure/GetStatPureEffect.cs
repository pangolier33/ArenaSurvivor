using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Stats.Containers.Graphs;
using UnityEngine;

namespace Bones.Gameplay.Effects.Pure
{
	public abstract class GetStatPureEffect<T> : PureEffect
	{
		[SerializeField] private StatName _name;
		protected sealed override void Invoke(ITrace trace) => Invoke(trace, trace.GetStatValue<T>(_name));
		protected abstract void Invoke(ITrace trace, T value);
	}
}