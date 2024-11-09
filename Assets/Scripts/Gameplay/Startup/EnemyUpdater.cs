using System;
using System.Collections.Generic;
using Bones.Gameplay.Entities;
using Bones.Gameplay.Map;
using Railcar.Time;
using UnityEngine;
using Zenject;

namespace Bones.Gameplay.Startup
{
	public class EnemyUpdater : MonoBehaviour, IInitializable, IDisposable
	{
		private IStopwatch _stopwatch;
		private IPositionTracker _tracker;
		private IEnumerable<Enemy> _enemies;
		private IDisposable _subscription;

		public void Initialize() => _subscription = _stopwatch.Observe(OnMoved);
		public void Dispose() => _subscription.Dispose();

		private void OnMoved(float timeDelta)
		{
			var playerPosition = _tracker.Property.Value;
			foreach (var enemy in _enemies)
			{
				var delta = playerPosition - (Vector2)enemy.Position;
				if (enemy.HandleDistance(delta.magnitude))
				{
					enemy.Dispose();
					continue;
				}

				if (!enemy.IsDead)
					enemy.Move(delta, playerPosition);
			}
		}
		
		[Inject]
		private void Inject([Inject(Id = TimeID.Fixed)] IStopwatch time, IPositionTracker tracker, IEnumerable<Enemy> enemies)
		{
			_stopwatch = time;
			_tracker = tracker;
			_enemies = enemies;
		}
	}
}