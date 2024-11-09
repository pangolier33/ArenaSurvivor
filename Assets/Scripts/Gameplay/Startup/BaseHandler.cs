using System;
using Railcar.Time;
using Zenject;

namespace Bones.Gameplay.Startup
{
    public abstract class BaseHandler : IDisposable, IInitializable
    {
        private readonly IStopwatch _stopwatch;
        private IDisposable _timeSubscription;

        protected BaseHandler(IStopwatch stopwatch)
        {
            _stopwatch = stopwatch;
        }
        
        public void Initialize()
        {
            _timeSubscription = _stopwatch.Observe(OnUpdated);
        }
        
        public void Dispose()
        {
            _timeSubscription.Dispose();
        }

        protected abstract void OnUpdated(float deltaTime);
    }
}