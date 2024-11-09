using System;
using Bones.Gameplay.Events.Callbacks.Templates.Base;
using UnityEngine;
using Zenject;
using UniRx;

namespace Bones.Gameplay.Map
{
    public class MapGenerator : MonoBehaviour, IInitializable, IDisposable, IInjectable
    {
        [SerializeField] private Renderer _tilePrefab;
        [SerializeField] private Transform _parent;
        [SerializeField] private Vector2Int _gridSize;

        private Vector2 _tileSize;
        private IDisposable _trackerSubscription;
        private IPositionTracker _tracker;
        
        public void Initialize()
        {
            _tileSize = _tilePrefab.bounds.size;
            for (var y = -_gridSize.y; y <= _gridSize.y; y++)
            {
                var worldY = _tileSize.y * (y - .5f);
                for (var x = -_gridSize.x; x <= _gridSize.x; x++)
                {
                    var worldX = _tileSize.x * (x - .5f);
                    var worldPosition = new Vector3(worldX, worldY);
                    Instantiate(_tilePrefab, worldPosition, Quaternion.identity, _parent);
                }
            }
            _trackerSubscription = _tracker.TrackShifting(_tileSize).Subscribe(OnNext);
        }
        
        public void Dispose()
        {
            _trackerSubscription.Dispose();
        }
        
        private void OnNext(Vector2Int position)
        {
            _parent.Translate(_tileSize * -position);
        }

        [Inject]
        private void Inject(IPositionTracker tracker)
        {
            _tracker = tracker;
        }
    }
}