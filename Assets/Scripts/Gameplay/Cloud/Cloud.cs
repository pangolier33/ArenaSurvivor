using Bones.Gameplay.Map;
using Railcar.Time;
using UnityEngine;
using Zenject;

namespace Bones.Gameplay.Cloud
{
    public class Cloud : MonoBehaviour
    {
        [SerializeField] private Renderer _tilePrefab;
        [SerializeField] private Vector2Int _size;
        [SerializeField, Range(0, 1.0f)] private float _parallaxCoefficient;
        [SerializeField] private Vector2 _velocity;
        
        private IPositionTracker _tracker;
        private ParallaxTileMover _parallaxTileMover;
        private IStopwatch _stopwatch;

        [Inject]
        private void Inject([Inject(Id = TimeID.Fixed)] IStopwatch stopwatch, IPositionTracker positionTracker)
        {
            _stopwatch = stopwatch;
            _tracker = positionTracker;
        }
        
        private void Awake()
        {
            _parallaxTileMover = new(_size, _tracker, _stopwatch, _tilePrefab, transform, _velocity, _parallaxCoefficient);
        }

        private void OnDestroy()
        {
            _parallaxTileMover.Dispose();
        }
    }
}