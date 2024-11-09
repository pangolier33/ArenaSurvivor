using Bones.Gameplay.Events.Args;
using Bones.Gameplay.Events.Callbacks.Templates.Base;
using Bones.Gameplay.Factories.Pools;
using Bones.Gameplay.Factories.Pools.Base;
using System;
using UnityEngine;
using Zenject;

namespace Bones.Gameplay.Events.Callbacks.Subscribers
{
	public abstract class AudioCallbackSubscriber<T> : CallbackSubscriber<T>
		where T : IPositionalMessageArgs
	{
		[SerializeField] private AudioInfo _info;

		private IPool<AudioInfo, AudioSource> _pool;

		protected override IObserver<T> GetCallback()
		{
			return new AudioCallback<T>(_pool, _info);
		}

		[Inject]
		private void Inject(IPool<AudioInfo, AudioSource> pool)
		{
			_pool = pool;
		}
	}
}