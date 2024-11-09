using System;
using System.Collections;
using Bones.Gameplay.Effects.Provider;
using UnityEngine;

namespace Bones.Gameplay.Effects.Transitive.Width
{
    [Serializable]
    public class LimitedRetrievingEffectWrapper : TransitiveEffect
    {
        [SerializeField] private int _limit;
        
        protected override void Invoke(IMutableTrace trace, IEffect next)
        {
            var count = 0;
            var enumerator = trace.Get<IEnumerable>().GetEnumerator();
            
            try
            {
                enumerator.Reset();
            }
            catch (NotSupportedException) { }

            while (enumerator.MoveNext() && count < _limit)
            {
                next.Invoke(trace.Add(enumerator.Current));
                count++;
            }
            
            if (enumerator is IDisposable disposable)
                disposable.Dispose();
        }
    }
}