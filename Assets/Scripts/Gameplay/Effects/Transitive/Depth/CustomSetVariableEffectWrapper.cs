using Bones.Gameplay.Effects.Provider;
using UnityEngine;

namespace Bones.Gameplay.Effects.Transitive.Depth
{
    public abstract class CustomSetVariableEffectWrapper<T> : SetVariableEffectWrapper<T>
    {
        [SerializeField] private T _value;

        protected sealed override T GetValue(ITrace provider) => _value;
    }
}