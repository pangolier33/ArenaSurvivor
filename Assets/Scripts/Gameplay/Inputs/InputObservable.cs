using System;
using Bones.Gameplay.Utils;
using Railcar.Time;
using UniRx;
using UnityEngine;
using Zenject;

namespace Bones.Gameplay.Inputs
{
	public class InputObservable : IDirectionalInput, IInitializable, IDisposable
	{
		private readonly ReactiveProperty<Vector2> _direction = new();
		private readonly ReactiveProperty<float> _speedModifier = new();
		private readonly IStopwatch _stopwatch;
		private readonly IDirectionalInput _wrappedInput;
		private IDisposable _subscription;

		public InputObservable([Inject(Id = TimeID.Fixed)] IStopwatch stopwatch, IDirectionalInput wrappedInput)
		{
			_stopwatch = stopwatch;
			_wrappedInput = wrappedInput;
		}
        
		Vector2 IDirectionalInput.Direction => _direction.Value;
		float IDirectionalInput.JoySpeedModifier => _speedModifier.Value;
		public IReadOnlyReactiveProperty<Vector2> Direction => _direction;
		public IReadOnlyReactiveProperty<Vector2> NotNullDirection =>
			_direction.Where(x => !x.Approximately(Vector2.zero));

		public void Initialize()
		{
			_subscription = _stopwatch.Observe(OnUpdated);
		}

		public void Dispose()
		{
			_subscription.Dispose();
		}
        
		private void OnUpdated(float _)
		{
			_direction.Value = _wrappedInput.Direction;
			_speedModifier.Value = _wrappedInput.JoySpeedModifier;
		}
	}
}