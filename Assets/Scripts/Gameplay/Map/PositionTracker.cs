using System;
using Railcar.Time;
using UniRx;
using UnityEngine;

namespace Bones.Gameplay.Map
{
	public class PositionTracker : IDisposable, IPositionTracker
    {
        private readonly ReactiveProperty<Vector2> _position;
        private readonly ReactiveProperty<Vector2> _forcedProperty = new();
        private readonly Transform _target;
        private readonly IDisposable _subscription;

        public PositionTracker(Transform target, IStopwatch stopwatch)
        {
            _position = new(target.position);
            _target = target;
            _subscription = stopwatch.Observe(OnUpdated);
        }

        public Vector2 Velocity { get; private set; }
        public IReadOnlyReactiveProperty<Vector2> Property => _position;
        public IReadOnlyReactiveProperty<Vector2> ForcedProperty => _forcedProperty;

        private void OnUpdated(float deltaTime)
        {
            var position = _target.position;
            Velocity = _position.Value - (Vector2)position;
            _forcedProperty.SetValueAndForceNotify(position);
            _position.Value = position;
        }

        public void Dispose()
        {
            _subscription.Dispose();
            _position.Dispose();
        }
    }
}