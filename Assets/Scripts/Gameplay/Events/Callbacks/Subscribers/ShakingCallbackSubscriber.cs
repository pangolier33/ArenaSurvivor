using Bones.Gameplay.Events.Args;
using Bones.Gameplay.Events.Callbacks.Templates.Base;
using DG.Tweening;
using System;
using UnityEngine;

namespace Bones.Gameplay.Events.Callbacks.Subscribers
{
	public class ShakingCallbackSubscriber : CallbackSubscriber<BossSpawnedArgs>, IObserver<BossSpawnedArgs>
	{
		[SerializeField] private Transform _target;
		[SerializeField] private float _duration;
		[SerializeField] private float _strength;
		[SerializeField] private int _vibrato;
		[SerializeField] private float _randomness;
		[SerializeField] private bool _fadeOut;
		[SerializeField] private ShakeRandomnessMode _randomnessMode;

		protected override IObserver<BossSpawnedArgs> GetCallback()
		{
			return this;
		}

		public void OnNext(BossSpawnedArgs value)
		{
			_target.DOShakePosition(
				_duration,
				strength: _strength,
				vibrato: _vibrato,
				randomness: _randomness,
				fadeOut: _fadeOut,
				randomnessMode: _randomnessMode
			);
		}

		public void OnCompleted()
		{
		}

		public void OnError(Exception error)
		{
			throw error;
		}

	}
}