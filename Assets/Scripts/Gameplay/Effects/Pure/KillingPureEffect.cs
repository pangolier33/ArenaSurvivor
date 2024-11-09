using System;
using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Effects.Transitive.Depth;

namespace Bones.Gameplay.Effects.Pure
{
    [Serializable]
    public sealed class KillingPureEffect : PureEffect
    {
        protected override void Invoke(ITrace trace)
        {
            var enemy = trace.Get<IPositionSource>().Stats;
            enemy.Kill();
        }
    }
}