using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Effects.Provider.Variables;
using UnityEngine;

namespace Bones.Gameplay.Effects.Transitive.Depth
{
    public abstract class BinaryOperationTransitiveEffect<TFirst, TSecond, TResult> : TransitiveEffect
    {
        [SerializeField] private string _firstID;
        [SerializeField] private string _secondID;
        [SerializeField] private string _resultID;

        protected sealed override void Invoke(IMutableTrace trace, IEffect next)
        {
            var first = trace.GetVariable<TFirst>(_firstID);
            var second = trace.GetVariable<TSecond>(_secondID);
            var result = Calculate(first, second);
            var variable = new MutableVariable<TResult>(_resultID, result);
            trace = trace.Add(variable);
            next.Invoke(trace);
        }

        protected abstract TResult Calculate(TFirst arg1, TSecond arg2);
    }
}