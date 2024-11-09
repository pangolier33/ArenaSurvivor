using System;
using Bones.Gameplay.Effects.Provider;
using UnityEngine;

namespace Bones.Gameplay.Effects.Transitive.Width
{
    [Serializable]
    public class IfElseTransitiveEffect : IEffect
    {
        [SerializeField] private string _id;
        [SerializeReference] private IEffect _trueEffect = default!;
        [SerializeReference] private IEffect _falseEffect = default!;
        
        public void Invoke(IMutableTrace trace)
        {
            var condition = trace.GetVariable<bool>(_id);
            
            if (condition)
                _trueEffect.Invoke(trace);
            else
                _falseEffect.Invoke(trace);
        }
    }
}