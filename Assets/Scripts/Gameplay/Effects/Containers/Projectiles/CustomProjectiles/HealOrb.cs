using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Effects.Transitive.Depth;
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
    public class HealOrb : ProjectileEffectContainer
	{
		enum State
		{ 
			ToTarget,
			ToPlayer,
		}

		public AnimationCurve velocityCurve;

		private IStopwatch _time;
		private IDisposable _subscription;
		private float _distance;
		private float _currentDistance = 0.0f;
		private Vector3 _target;
		private Player _player;
		private float _projectileSpeed = 0.0f;
		private float _healthHealed = 0.0f;
		private State state = State.ToTarget;
		private EnemiesCollectionCache _enemies;
		private int _hitCount = 0;

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
			state = State.ToTarget;
			_hitCount = 0;
			_currentDistance = 0.0f;

			//Set target
			_target = Settings.Trace.Get<Enemy>().Position;
			_player = Settings.Trace.Get<Player>();
			Vector3 d = (_target - _player.transform.position).normalized * Settings.Trace.GetStatValue<Value>(StatName.Distance);
			_target = _player.transform.position + d;
			_distance = d.magnitude;

			//Set initial position
			Vector2 tp = Settings.Trace.GetStatValue<Pair>(StatName.SpawnPosition);
			_projectileSpeed = Settings.Trace.GetStatValue<Value>(StatName.ProjectileSpeed);
			_healthHealed = Settings.Trace.GetStatValue<Value>(StatName.Health);
			transform.position = tp;
		}

		void SpellUpdate(float delta)
		{
			//Find target
			Vector3 target;
			if (state == State.ToTarget)
				target = _target;
			else
				target = _player.Position;

			//Move to target
			Vector3 position = transform.position;
			var d = target - position;
			float projectileSpeed = _projectileSpeed;
			if (state == State.ToTarget)
				projectileSpeed *= velocityCurve.Evaluate(0.5f * _currentDistance / _distance);
			else
				projectileSpeed *= velocityCurve.Evaluate(0.5f + 0.5f * _currentDistance / _distance);
			if (d.magnitude > delta * projectileSpeed)
				d = d.normalized * delta * projectileSpeed;
			else 
			{
				if (state == State.ToPlayer)
				{
					//Dispose when near player
					((IAddStat<Value>)_player.Stats.Get(StatName.Health)).Add(new Value(_healthHealed * _hitCount));
					Dispose();
				}
				else
				{
					state = State.ToPlayer;
					_currentDistance = 0.0f;
				}
				
			}

			transform.position = position + d;
			_currentDistance += d.magnitude;
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			GameObject go = other.gameObject;

			if (!IsSpawned)
				return;

			if (go.TryGetComponent<Enemy>(out var enemy) && enemy.IsSpawned)
			{
				((ISubtractStat<Value>)enemy.Stats.Get(StatName.OverallHealth)).Subtract(Settings.Trace.GetStatValue<Value>(StatName.Damage));
				++_hitCount;
			}
		}
    }
}
