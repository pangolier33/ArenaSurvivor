using System;
using System.Collections;
using Bones.Gameplay.Effects.Provider;

namespace Bones.Gameplay.Effects.Transitive.Width
{
    [Serializable]
    public class AllRetrievingEffectWrapper : TransitiveEffect
    {
        protected override void Invoke(IMutableTrace trace, IEffect next)
        {
            foreach (var arg in trace.Get<IEnumerable>())
            {
                next.Invoke(trace.Add(arg));
            }
        }
    }
}