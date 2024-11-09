using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Entities;
using Bones.Gameplay.Factories.Pools.Mono;
using Bones.Gameplay.Players;
using Bones.Gameplay.Stats.Containers.Graphs;
using Bones.Gameplay.Stats.Containers;
using Bones.Gameplay.Stats.Units;
using Railcar.Time;
using System;
using UnityEngine;
using Zenject;

//Example class for custom projectiles
namespace Bones.Gameplay.Effects.Containers.Projectiles
{
	public class CustomProjectile : MonoPoolBridge<ProjectileSettings, CustomProjectile>/*, IProjectileHandle, ITriggerEnterBridge*/
	{
		private IStopwatch _time;
		private IDisposable _subscription;
		private Vector3 _target;
		private Player _player;
		private float _projectileSpeed = 0.0f;
		private float _healthHealed = 0.0f;
		private EnemiesCollectionCache _enemies;

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

			//Set target
			_target = Settings.Trace.Get<Enemy>().Position;
			_player = Settings.Trace.Get<Player>();
			Vector3 d = (_target - _player.transform.position).normalized * Settings.Trace.GetStatValue<Value>(StatName.Distance);
			_target = _player.transform.position + d;

			//Set initial position
			_projectileSpeed = Settings.Trace.GetStatValue<Value>(StatName.ProjectileSpeed);
			_healthHealed = Settings.Trace.GetStatValue<Value>(StatName.Health);
		}

		void SpellUpdate(float delta)
		{
			//Find target
			Vector3 target;
			{
				/*if (_target == null || !_target.IsSpawned)
				{
					//Find new target
					var enemy = _enemies.GetSorted(_player).GetEnumerator().Current;
					if (enemy != null)
						_target = enemy;
				}*/
			}
			target = _player.Position;

			//Move to target
			Vector3 position = transform.position;
			var d = target - position;
			if (d.magnitude > delta * _projectileSpeed)
				d = d.normalized * delta * _projectileSpeed;
			else
			{
				{
					//Heal player
					((IAddStat<Value>)_player.Stats.Get(StatName.OverallHealth)).Add(new Value(_healthHealed));
					//Dispose when near player
					Dispose();
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
			}
		}
	}
}
