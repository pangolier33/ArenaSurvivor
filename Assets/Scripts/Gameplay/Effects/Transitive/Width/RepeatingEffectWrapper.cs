using Bones.Gameplay.Effects.Provider;
using UnityEngine;

namespace Bones.Gameplay.Effects.Transitive.Width
{
    public class RepeatingEffectWrapper : TransitiveEffect
    {
        [SerializeField] private int _repeats;
        
        protected override void Invoke(IMutableTrace trace, IEffect next)
        {
            for (var i = 0; i < _repeats; i++)
                next.Invoke(trace);
        }
    }
}