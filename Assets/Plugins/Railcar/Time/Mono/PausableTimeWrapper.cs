using System;
using Railcar.Time.Subscriptions;
using UnityEngine;

namespace Railcar.Time.Mono
{
	public class PausableTimeWrapper : MonoBehaviour, ITimeLord
	{
		[SerializeField] private ulong _skipsCount;
		[SerializeField] private float _timeBooster;
		
		private readonly TimeFacade _fixedTime = new();
		private readonly TimeFacade _frameTime = new();
		private readonly TimeFacade _lateFrameTime = new();

		private bool _doesFlow;
		private ulong _ticks;
		
		public bool DoesFlow
		{
			get => _doesFlow;
			set
			{
				_doesFlow = value;
				FlowingUpdated?.Invoke(this, _doesFlow);
			}
		}

		public event EventHandler<bool> FlowingUpdated;
		public TimeFacade FixedTime => _fixedTime;
		public TimeFacade FrameTime => _frameTime;
		public TimeFacade LateFrameTime => _lateFrameTime;

		private void FixedUpdate()
		{
			if (++_ticks % _skipsCount == 0)
				return;
			UpdateTime(_fixedTime, UnityEngine.Time.fixedDeltaTime * _timeBooster);
		}

		private void Update()
		{
			UpdateTime(_frameTime, UnityEngine.Time.deltaTime);
		}

		private void LateUpdate()
		{
			UpdateTime(_lateFrameTime, UnityEngine.Time.deltaTime);
		}

		private void OnDestroy()
		{
			_fixedTime.Dispose();
			_frameTime.Dispose();
			_lateFrameTime.Dispose();
		}

		private void UpdateTime(TimeFacade timeObservable, float timeDelta)
		{
			if (!DoesFlow)
				return;

			timeObservable.Update(timeDelta);
		}
	}
}