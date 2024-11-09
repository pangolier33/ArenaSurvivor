using System;
using System.Collections.Generic;
using Bones.Gameplay.Entities;
using Bones.Gameplay.Events.Args;
using Bones.Gameplay.Map;
using UniRx;
using UnityEngine;
using Zenject;

namespace Bones.UI.Presenters
{
    public class PoiContainerPresenter : MonoBehaviour, IInitializable, IDisposable
    {
        private readonly CompositeDisposable _brokerSubscriptions = new();
        private readonly Dictionary<Enemy, PrefabPresenter> _viewsByEnemy = new();
        private IMessageReceiver _receiver;
        private IPositionTracker _targetTracker;
        
        public void Initialize()
        {
            _receiver.Receive<EnemySpawnedArgs>()
                .Select(x => x.Subject)
                .Where(IsPoi)
                .Subscribe(OnPoiSpawned)
                .AddTo(_brokerSubscriptions);

            _receiver.Receive<EnemyDiedArgs>()
                .Select(x => x.Subject)
                .Subscribe(OnPoiDespawned)
                .AddTo(_brokerSubscriptions);
        }
        
        [Inject]
        private void Inject(IMessageReceiver receiver, IPositionTracker targetTracker)
        {
            _receiver = receiver;
            _targetTracker = targetTracker;
        }

        private static bool IsPoi(Enemy enemy)
        {
            return enemy.TryGetComponent(out Poi _);
        }
        
        private void OnPoiSpawned(Enemy enemy)
        {
            var markerPrefab = enemy.GetComponent<Poi>().MarkerPrefab;
            var observable = _targetTracker.ForcedProperty.Select(_ => (Vector2)(enemy.transform.position));
            var marker = markerPrefab.Spawn(observable, transform);
            _viewsByEnemy.Add(enemy, marker);
        }

        private void OnPoiDespawned(Enemy enemy)
        {
            if (!_viewsByEnemy.TryGetValue(enemy, out var view))
                return;
            view.Dispose();
            _viewsByEnemy.Remove(enemy);
        }

        public void Dispose()
        {
            _brokerSubscriptions?.Dispose();
            foreach (var (_, view) in _viewsByEnemy)
                view.Dispose();
            _viewsByEnemy.Clear();
        }
    }
}