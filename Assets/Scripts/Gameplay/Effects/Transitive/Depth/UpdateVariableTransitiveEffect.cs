using Bones.Gameplay.Effects.Provider;
using UnityEngine;

namespace Bones.Gameplay.Effects.Transitive.Depth
{
    public abstract class UpdateVariableTransitiveEffect<T> : TransitiveEffect
    {
        [SerializeField] private string _sourceId;
        [SerializeField] private string _providerId;

        protected override void Invoke(IMutableTrace trace, IEffect next)
        {
            trace.SetVariable(_sourceId, trace.GetVariable<T>(_providerId));
            next.Invoke(trace);
        }
    }
}