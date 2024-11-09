using Bones.Gameplay.Effects.Provider;

namespace Bones.Gameplay.Effects.Pure
{
    public class NopPureEffect : PureEffect
    {
        protected override void Invoke(ITrace trace)
        {
            return;
        }
    }
}