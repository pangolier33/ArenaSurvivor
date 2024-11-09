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
using Bones.Gameplay.Stats.Utils;

namespace Bones.Gameplay.Effects.Containers.Projectiles
{
	public class SelfDamage : ProjectileEffectContainer
	{
		public AnimationCurve sizeCurve;
		public float fadeTime = 0.2f;
		public LayerMask layerMask;
		public bool followEnemy = true;

		IStopwatch _time;
		IDisposable _subscription;
		Player _player;
		Enemy _enemy;
		float _timeRemain;
		float _attackTimeRamain;

		float _area;
		float _damage;
		float _attackCooldown;
		float _duration;


		[Inject]
		private void Inject([Inject(Id = TimeID.Fixed)] IStopwatch time)
		{
			_time = time;
		}

		protected override void OnDespawned()
		{
			_subscription?.Dispose();
			_subscription = null;
		}

		protected override void OnSpawned()
		{
			_subscription = _time.Observe(delta => SpellUpdate(delta));

			_area = Settings.Trace.GetStatValue<Value>(StatName.Area);
			_damage = Settings.Trace.GetStatValue<Value>(StatName.Damage);
			_attackCooldown = Settings.Trace.GetStatValue<Value>(StatName.Delta);
			_duration = Settings.Trace.GetStatValue<Value>(StatName.Duration);
			
			_timeRemain = _duration;
			_attackTimeRamain = _attackCooldown;

			_player = Settings.Trace.Get<Player>();

			//Set initial position
			_enemy = Settings.Trace.Get<Enemy>();
			transform.position = _enemy.Position;
			transform.localScale = new Vector3(_area, _area, _area);
		}

		void SpellUpdate(float delta)
		{
			_timeRemain -= delta;

			if (_timeRemain < -0.5f)
			{
				Dispose();
				return;
			}

			if ( _timeRemain <= 0 )
				return;

			if (_enemy != null)
			{
				transform.position = _enemy.Position;
				if (_enemy.IsDead || !_enemy.IsSpawned)
					_enemy = null;
			}

			_attackTimeRamain -= delta;
			if (_attackTimeRamain <= 0 )
			{
				_attackTimeRamain = _attackCooldown;

				ContactFilter2D filter = new ContactFilter2D();
				filter.SetLayerMask(layerMask);
				Collider2D[] enemies = new Collider2D[1000];

				int num = Physics2D.OverlapCollider (GetComponent<Collider2D>(), filter, enemies);
				foreach ( var collider in enemies )
				{
					if (collider.gameObject.TryGetComponent(out Enemy enemy))
					{
						float hp = _damage * enemy.Stats.GetValue<Value>(StatName.OverallHealth);
						if (hp < 1.0f)
							hp = 1.0f;
						((ISubtractStat<Value>)enemy.Stats.Get(StatName.OverallHealth)).Subtract(new Value(hp));

					}

					if (--num <= 0)
						break;
				}
			}
		}
	}
}
