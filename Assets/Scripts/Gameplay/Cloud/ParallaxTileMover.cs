using System;
using Railcar.Time;
using UniRx;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Bones.Gameplay.Map
{
	public class ParallaxTileMover : IDisposable
    {
        private readonly Transform _parent;
        private readonly Vector2 _tileSize;
        private readonly IPositionTracker _tracker;
        private readonly IDisposable _subscription;
        private readonly IStopwatch _time;
        private readonly float _parallaxCoefficient;
        private readonly Vector2 _velocity;
        private readonly Vector2 _startOffset;

        private float RelativeX => _tracker.Property.Value.x - _parent.position.x - _startOffset.x;
        private float RelativeY => _tracker.Property.Value.y - _parent.position.y - _startOffset.y;

        public ParallaxTileMover(Vector2Int gridSize, IPositionTracker tracker, IStopwatch time,
            Renderer rendererPrefab, Transform parent, Vector2 velocity, float parallaxCoefficient)
        {
            _parent = parent;
            _tracker = tracker;
            _velocity = velocity;
            _parallaxCoefficient = parallaxCoefficient;
            _tileSize = rendererPrefab.bounds.size;
            _startOffset = _tracker.Property.Value - (Vector2)_parent.position;
            for (var y = -gridSize.y; y <= gridSize.y; y++)
            {
                var worldY = _tileSize.y * (y - .5f);
                for (var x = -gridSize.x; x <= gridSize.x; x++)
                {
                    var worldX = _tileSize.x * (x - .5f);
                    var worldPosition = new Vector3(worldX, worldY);
                    Object.Instantiate(rendererPrefab, worldPosition, Quaternion.identity, parent);
                }
            }

            _subscription = new CompositeDisposable(
                tracker.Property.Subscribe(OnTrackerMoved),
                time.Observe(OnUpdated));
        }

        public void Dispose()
        {
            _subscription.Dispose();
        }

        private void OnTrackerMoved(Vector2 position)
        {
            Translate(_tracker.Velocity * _parallaxCoefficient);
        }

        private void OnUpdated(float deltaTime)
        {
            Translate(new Vector2(deltaTime * _velocity.x, deltaTime * _velocity.y));
        }

        private void Translate(Vector2 translation)
        {
            _parent.Translate(translation);
            while (Mathf.Abs(RelativeX) > _tileSize.x)
            {
                _parent.Translate(_tileSize.x * Math.Sign(RelativeX), 0, 0);
            }

            while (Mathf.Abs(RelativeY) > _tileSize.y)
            {
                _parent.Translate(0, _tileSize.y * Math.Sign(RelativeY), 0);
            }
        }
    }
}