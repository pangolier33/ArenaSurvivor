using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Entities;
using Bones.Gameplay.Players;
using Bones.Gameplay.Stats.Units;
using Railcar.Time;
using System;
using Bones.Gameplay.Stats.Containers.Graphs;
using UnityEngine;
using Zenject;
using Bones.Gameplay.Stats.Containers;

namespace Bones.Gameplay.Effects.Containers.Projectiles
{
	public class Ricochet : ProjectileEffectContainer
	{
		private IStopwatch _time;
		private IDisposable _subscription;
		private float _distance;
		private Enemy _target;
		private Player _player;
		private float _projectileSpeed = 0.0f;
		private EnemiesCollectionCache _enemies;
		private int _hitCount = 0;
		private float _shieldRegen = 0.0f;

		[Inject]
		private void Inject([Inject(Id = TimeID.Fixed)] IStopwatch time, EnemiesCollectionCache enemies)
		{
			_time = time;
			_enemies = enemies;
		}

		protected override void OnDespawned()
		{
			_subscription?.Dispose();
			_subscription = null;
		}

		protected override void OnSpawned()
		{
			_subscription = _time.Observe(delta => SpellUpdate(delta));
			_hitCount = Settings.Trace.GetStatValue<Amount>(StatName.Count); ;
			_distance = Settings.Trace.GetStatValue<Value>(StatName.Distance);
			_projectileSpeed = Settings.Trace.GetStatValue<Value>(StatName.ProjectileSpeed);
			_shieldRegen = Settings.Trace.GetStatValue<Value>(StatName.ShieldRegen);

			//Set target
			_target = Settings.Trace.Get<Enemy>();
			_player = Settings.Trace.Get<Player>();

			//Set initial position
			//Vector2 tp = Settings.Trace.GetStatValue<Pair>(StatName.SpawnPosition);
			transform.position = _player.Position;
		}

		Enemy FindNewTarget()
		{
			float maxDistance = 0.0f;
			Vector3 pos = transform.position;
			Enemy res = null;
			foreach (var enemy in _enemies.GetSorted(_target, _distance))
			{
				if (enemy != null)
				{
					float d = (pos - enemy.transform.position).magnitude;
					if (d > maxDistance)
					{
						maxDistance = d;
						res = enemy;
					}
				}
			}

			return res;
		}

		void SpellUpdate(float delta)
		{
			//Find target
			Vector3 target;
			if (!_target.IsSpawned)
			{
				_target = FindNewTarget();
				if (_target == null)
				{
					Dispose();
					return;
				}
			}
			target = _target.Position;

			//Move to target
			Vector3 position = transform.position;
			var d = target - position;
			if (d.magnitude > delta * _projectileSpeed)
				d = d.normalized * delta * _projectileSpeed;
			else
			{
				_target = FindNewTarget();
				if (_target == null)
				{
					Dispose();
					return;
				}
			}

			transform.position = position + d;
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			GameObject go = other.gameObject;

			if (!IsSpawned)
				return;

			if (go.TryGetComponent<Enemy>(out var enemy) && enemy.IsSpawned)
			{
				((ISubtractStat<Value>)enemy.Stats.Get(StatName.OverallHealth)).Subtract(Settings.Trace.GetStatValue<Value>(StatName.Damage));

				if (enemy == _target)
				{
					((IAddStat<Value>)_player.Stats.Get(StatName.Shield)).Add(new Value(_shieldRegen));
					--_hitCount;
					if (_hitCount == 0)
					{
						Dispose();
						return;
					}
					else
					{
						_target = FindNewTarget();
						if (_target == null)
						{
							Dispose();
							return;
						}
					}
				}
			}
		}
    }
}
