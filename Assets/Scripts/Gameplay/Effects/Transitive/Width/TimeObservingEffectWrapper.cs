using System;
using Bones.Gameplay.Effects.Provider;
using Railcar.Time;
using Zenject;

namespace Bones.Gameplay.Effects.Transitive.Width
{
    [Serializable]
    public class TimeObservingEffectWrapper : TransitiveEffect
    {
        private IStopwatch _time;
        
        protected override void Invoke(IMutableTrace trace, IEffect next)
        {
            var subscription = _time.Observe(_ => next.Invoke(trace));
            trace.ConnectToClosest(subscription);
        }

        [Inject]
        private void Inject([Inject(Id = TimeID.Fixed)] IStopwatch time)
        {
            _time = time;
        }
    }
}