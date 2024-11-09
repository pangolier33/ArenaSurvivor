using System;
using Bones.Gameplay.Map;
using Railcar.Time;
using Zenject;

namespace Bones.Gameplay.Camera
{
	public class UpdatingCameraFollower : CameraFollower, IDisposable
	{
		private IDisposable _timeSubscription;
		private IPositionTracker _tracker;
		private IStopwatch _stopwatch;
        
		public override void Initialize()
		{
			base.Initialize();
			_timeSubscription = _stopwatch.Observe(OnUpdated);
		}
		public void Dispose()
		{
			_timeSubscription.Dispose();
		}

		private void OnUpdated(float deltaTime)
		{
			Follow(_tracker.Property.Value, deltaTime);            
		}

		[Inject]
		private void Inject(IPositionTracker tracker, [Inject(Id = TimeID.Fixed)] IStopwatch stopwatch)
		{
			_tracker = tracker;
			_stopwatch = stopwatch;
		}
	}
}