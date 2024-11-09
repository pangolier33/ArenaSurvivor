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
	public class Chains : ProjectileEffectContainer
	{
		enum State
		{ 
			Appearing,
			Rotation,
			Dissappearing,
		}

		public AnimationCurve appearScale;
		public AnimationCurve sideScale;
		public AnimationCurve rotationScale;
		public float fadeTime = 0.2f;

		private IStopwatch _time;
		private IDisposable _subscription;
		private float _distance;
		private float _speed;
		private float _duration;
		private Vector3 _target;
		private Enemy _enemy;
		private Player _player;
		private Transform _left;
		private Transform _right;
		
		private State _state = State.Appearing;
		private float _progress;


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
			_state = State.Appearing;
			_player = Settings.Trace.Get<Player>();
			_enemy = Settings.Trace.Get<Enemy>();
			_target = _enemy.Position;
			
			_distance = Settings.Trace.GetStatValue<Value>(StatName.Distance);
			_duration = Settings.Trace.GetStatValue<Value>(StatName.Duration);
			_speed = Settings.Trace.GetStatValue<Value>(StatName.ProjectileSpeed);
			_progress = _duration;

			_left = transform.Find("Left");
			_right = transform.Find("Right");

			var s = _left.transform.localScale;
			s.y = 0.001f;
			_left.transform.localScale = s;

			s = _right.transform.localScale;
			s.y = 0.001f;
			_right.transform.localScale = s;

			//Set rotaiton
			Vector3 d = _target - _player.transform.position;
			float angle = Vector2.SignedAngle(Vector2.up, d);
			transform.rotation = Quaternion.Euler(0, 0, angle);

			//Set initial position
			//Vector2 tp = Settings.Trace.GetStatValue<Pair>(StatName.SpawnPosition);			
			transform.position = _player.transform.position;
		}

		void SpellUpdate(float delta)
		{
			_progress -= delta * _speed;
			if (_progress < 0)
			{
				Dispose();
				return;
			}

			float fadeScale = 1.0f;
			if (_progress < fadeTime)
			{
				fadeScale = Mathf.Clamp01(_progress / fadeTime);
			}
			else if (_progress > _duration - fadeTime)
			{
				fadeScale = Mathf.Clamp01((_duration - _progress) / fadeTime);
			}

			fadeScale = appearScale.Evaluate(fadeScale);
			float sideScaleV = sideScale.Evaluate(_progress * 4);
			float rotationScaleV = _progress * 2;// + rotationScale.Evaluate(_progress * 2);
			
			var s = _left.transform.localScale;
			s.y = fadeScale * sideScaleV;
			_left.transform.localScale = s;

			s = _right.transform.localScale;
			s.y = fadeScale * sideScaleV;
			_right.transform.localScale = s;


			_left.transform.localRotation = Quaternion.Euler(0, 0, rotationScaleV * 180.0f);
			_right.transform.localRotation = Quaternion.Euler(0, 0, -rotationScaleV * 180.0f);

			transform.position = _player.transform.position;
		}

		public void OnChainsCollision(Collider2D other)
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
