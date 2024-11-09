using Bones.Gameplay.Effects.Provider;

namespace Bones.Gameplay.Effects.Pure
{
    public abstract class PureEffect : IEffect
    {
        public void Invoke(IMutableTrace trace)
        {
#if UNITY_EDITOR
            Invoke((ITrace)trace.Add(GetType().Name));
#else
            Invoke((ITrace)trace);
#endif
        }

        protected abstract void Invoke(ITrace trace);
    }
}