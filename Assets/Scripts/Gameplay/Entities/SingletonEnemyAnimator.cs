using System;
using Railcar.Time;
using UniRx;
using UnityEngine;
using Zenject;

namespace Bones.Gameplay.Entities
{
	public class SingletonEnemyAnimator : IDisposable
	{
		private readonly IDisposable _subscription;
		private readonly AnimationCurve _scalingCurve;
		private float _startSize;
		
		private SingletonEnemyAnimator([Inject(Id = TimeID.Frame)] IObservableClock clock, AnimationCurve scalingCurve, float startSize)
		{
			_scalingCurve = scalingCurve;
			_startSize = startSize;

			_subscription = clock.Current.Subscribe(OnUpdated);
		}

		public void Init(float startSize)
		{
			_startSize = startSize;
		}

		public float Scale { get; private set; }

		private void OnUpdated(float time)
		{
			Scale = _scalingCurve.Evaluate(time) * _startSize;
		}

		public void Dispose()
		{
			_subscription.Dispose();
		}
	}
}