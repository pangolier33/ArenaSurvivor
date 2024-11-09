using System;
using Bones.Gameplay.Entities;
using Bones.Gameplay.Experience;
using Bones.Gameplay.Factories.Pools;
using Bones.Gameplay.Factories.Pools.Base;
using Bones.Gameplay.Players;
using Cysharp.Threading.Tasks;
using Railcar.Time;
using UnityEngine;
using Zenject;

namespace Bones.Gameplay.Effects.Containers
{
	public class CircleOfExperience : Obstacle
	{
		[SerializeField] private Transform _filling;
		[SerializeField] private AudioSource _activatingSound;
		
		[SerializeField] private ParticleSystem _idleParticle;
		[SerializeField] private ParticleSystem _despawningVfx;
		[SerializeField] private AudioSource _idleSound;
		[SerializeField] private AudioInfo _obtainSound;
		
		[SerializeField] private int _steps;

		private Vector3 _fullScale;
		
		private int _stepsCap;
		
		private IDisposable _timeSubscription;
		private IStopwatch _stopwatch;
		
		private MultiplyingExperienceBank _expBank;
		private IPool<ParticleSystem> _particleSystemPool;
		private IPool<AudioInfo, AudioSource> _soundPool;

		protected sealed override async void OnSpawned()
		{
			_stepsCap = 0;
			_filling.localScale = Vector3.zero;

			await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
			_idleParticle.Play();
			_idleSound.Play();
		}

		protected sealed override void OnDespawned()
		{
			_idleSound.Stop();
		}
		
		protected sealed override void Awake()
		{
			base.Awake();
			_fullScale = _filling.localScale;
		}
		
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (!IsSpawned)
				return;
			if (!other.TryGetComponent(out Player _))
				return;

			_timeSubscription ??= _stopwatch.Observe(OnObtaining);
			
			_idleParticle.Stop();
			_activatingSound.Play();
		}

		private void OnTriggerExit2D(Collider2D other)
		{
			if (!other.TryGetComponent(out Player _))
				return;
			
			_activatingSound.Stop();
			_timeSubscription?.Dispose();
			_timeSubscription = null;
		}

		private void OnObtaining(float delta)
		{
			_expBank.ObtainPercent(1f / _steps, false);
			_filling.localScale = _fullScale * ((float)(++_stepsCap) / _steps);
			if (_stepsCap == _steps)
			{
				var vfx = _particleSystemPool.Create();
				vfx.transform.position = transform.position;
				vfx.Play();
				var source = _soundPool.Create(_obtainSound);
				source.transform.position = transform.position;
				source.Play();
				_timeSubscription.Dispose();
				_timeSubscription = null;
				Dispose();
			}
		}
		
		[Inject]
		private void Inject([Inject(Id = TimeID.Fixed)] IStopwatch time, MultiplyingExperienceBank expBank, IFactory<ParticleSystem, IPool<ParticleSystem>> particleSystemPool, IPool<AudioInfo, AudioSource> soundPool)
		{
			_stopwatch = time;
			_expBank = expBank;
			_particleSystemPool = particleSystemPool.Create(_despawningVfx);
			_soundPool = soundPool;
		}
	}
}