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
	public class MagicBolt : ProjectileEffectContainer
	{
		IStopwatch _time;
		IDisposable _subscription;
		Vector3 _target;
		Player _player;
		float _projectileSpeed = 0.0f;
		int _pierce;

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
			_player = Settings.Trace.Get<Player>();
			_target = Settings.Trace.Get<Enemy>().Position;

			//Set target
			int offset = 0;
			Settings.Trace.TryGetVariable<int>("Offset", out offset);

			Vector3 d = (_target - _player.transform.position).normalized;			
			float angle = Vector2.SignedAngle(Vector2.up, d);
			transform.rotation = Quaternion.Euler(0, 0, angle + offset * 10.0f);
			_target = _player.transform.position + transform.up * Settings.Trace.GetStatValue<Value>(StatName.Distance);

			//Set initial position
			_projectileSpeed = Settings.Trace.GetStatValue<Value>(StatName.ProjectileSpeed);
			_pierce = Settings.Trace.GetStatValue<Amount>(StatName.Piercing);
			transform.position = _player.transform.position;
		}

		void SpellUpdate(float delta)
		{
			//Move to target
			Vector3 position = transform.position;
			var d = _target - position;
			if (d.magnitude > delta * _projectileSpeed)
				d = d.normalized * delta * _projectileSpeed;
			else 
			{
				Dispose();
				return;
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
				--_pierce;
				if (_pierce < 0)
				{
					Dispose();
					return;
				}
			}
		}
    }
}
