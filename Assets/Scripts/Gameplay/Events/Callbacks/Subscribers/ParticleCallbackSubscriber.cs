using Bones.Gameplay.Events.Args;
using Bones.Gameplay.Events.Callbacks.Templates.Base;
using Bones.Gameplay.Factories.Pools.Base;
using System;
using UnityEngine;
using Zenject;

namespace Bones.Gameplay.Events.Callbacks.Subscribers
{
	public class ParticleCallbackSubscriber : CallbackSubscriber<EnemyDiedArgs>
	{
		[SerializeField] private ParticleSystem _prefab;

		private IFactory<ParticleSystem, IPool<ParticleSystem>> _poolFactory;

		protected override IObserver<EnemyDiedArgs> GetCallback()
		{
			return new VfxCallback<EnemyDiedArgs>(_poolFactory.Create(_prefab));
		}

		[Inject]
		private void Inject(IFactory<ParticleSystem, IPool<ParticleSystem>> poolFactory)
		{
			_poolFactory = poolFactory;
		}
	}
}
