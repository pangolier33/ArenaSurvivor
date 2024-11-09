using Bones.Gameplay.Effects.Provider;
using UnityEngine;

namespace Bones.Gameplay.Effects.Pure
{
    public abstract class GetVariablePureEffect<T> : PureEffect
    {
        [SerializeField] private string _id;
        
        protected sealed override void Invoke(ITrace trace)
        {
            Invoke(trace, trace.GetVariable<T>(_id));
        }

        protected abstract void Invoke(ITrace trace, T value);
    }
}