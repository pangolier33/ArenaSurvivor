using System;
using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Effects.Provider.Branches;

namespace Bones.Gameplay.Effects.Pure
{
    [Serializable]
    public sealed class BranchDisconnectingPureEffect : PureEffect
    {
        protected override void Invoke(ITrace trace)
        {
            var disposable = trace.Get<IBranch>();
            disposable.Dispose();
        }
    }
}