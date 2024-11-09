using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Effects.Provider.Variables;
using UnityEngine;

namespace Bones.Gameplay.Effects.Transitive.Depth
{
    public abstract class SetVariableEffectWrapper<T> : TransitiveEffect
    {
        [SerializeField] private string _id;

        protected override void Invoke(IMutableTrace trace, IEffect next)
        {
            next.Invoke(trace.Add(new MutableVariable<T>(_id, GetValue(trace))));
        }

        protected abstract T GetValue(ITrace provider);
    }
}