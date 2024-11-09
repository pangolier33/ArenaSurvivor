using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Effects.Provider.Variables;
using UnityEngine;

namespace Bones.Gameplay.Effects.Transitive.Depth
{
    public abstract class UnaryOperationTransitiveEffect<TFrom, TResult> : TransitiveEffect
    {
        [SerializeField] private string _fromID;
        [SerializeField] private string _resultID;

        protected sealed override void Invoke(IMutableTrace trace, IEffect next)
        {
            var first = trace.GetVariable<TFrom>(_fromID);
            var result = Calculate(first);
            var variable = new MutableVariable<TResult>(_resultID, result);
            trace = trace.Add(variable);
            next.Invoke(trace);
        }

        protected abstract TResult Calculate(TFrom arg);
    }
}