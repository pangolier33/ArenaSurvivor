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
	public class Shock : ProjectileEffectContainer
	{
		private IStopwatch _time;
		private IDisposable _subscription;
		private float _totalDuration = 0.0f;
		private float _colliderDuration = 0.0f;
		private Player _player;
		private EnemiesCollectionCache _enemies;
		private float _impulse = 0.0f;
		Collider2D _collider;

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
			_colliderDuration = Settings.Trace.GetStatValue<Value>(StatName.Duration);
			_totalDuration = _colliderDuration * 2.0f;
			_collider = gameObject.GetComponent<Collider2D>();
			if (_collider)
				_collider.enabled = true;
			_impulse = Settings.Trace.GetStatValue<Value>(StatName.Impulse);
			_player = Settings.Trace.Get<Player>();

			//Set target
			Vector3 target = Settings.Trace.Get<Enemy>().Position;
			Vector3 d = (target - _player.transform.position);
			float angle = Vector2.SignedAngle(Vector2.up, d);
			transform.rotation = Quaternion.Euler(0, 0, angle);

			//Set initial position
			transform.position = _player.transform.position;
		}

		void SpellUpdate(float delta)
		{
			if (_colliderDuration >= 0 && (_colliderDuration - delta) < 0)
			{
				if (_collider)
					_collider.enabled = false;
			}

			_totalDuration -= delta;
			_colliderDuration -= delta;

			if (_totalDuration < 0)
			{
				Dispose();
			}

			if (_player)
				transform.position = _player.transform.position;
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			GameObject go = other.gameObject;

			if (!IsSpawned)
				return;

			if (go.TryGetComponent<Enemy>(out var enemy) && enemy.IsSpawned)
			{
				((ISubtractStat<Value>)enemy.Stats.Get(StatName.OverallHealth)).Subtract(Settings.Trace.GetStatValue<Value>(StatName.Damage));
				enemy.AddImpulse(_player.transform.position, _impulse);
			}
		}
    }
}
