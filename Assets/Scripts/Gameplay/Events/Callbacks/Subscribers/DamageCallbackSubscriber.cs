using Bones.Gameplay.Entities;
using Bones.Gameplay.Events.Args;
using Bones.Gameplay.Events.Callbacks.Templates.Base;
using Bones.Gameplay.Factories.Pools.Base;
using System;
using UnityEngine;
using Zenject;

namespace Bones.Gameplay.Events.Callbacks.Subscribers
{
	public class DamageCallbackSubscriber : CallbackSubscriber<EnemyDamagedArgs>
	{
		[SerializeField] private Damage _prefab;

		private IFactory<Damage, IPool<EnemyDamagedArgs, Damage>> _poolFactory;

		protected override IObserver<EnemyDamagedArgs> GetCallback()
		{
			return new DamageCallback(_poolFactory.Create(_prefab));
		}

		[Inject]
		private void Inject(IFactory<Damage, IPool<EnemyDamagedArgs, Damage>> poolFactory)
		{
			_poolFactory = poolFactory;
		}
	}
}
