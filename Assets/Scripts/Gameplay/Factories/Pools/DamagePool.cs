using Bones.Gameplay.Entities;
using Bones.Gameplay.Events.Args;
using Bones.Gameplay.Factories.Pools.Base;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Bones.Gameplay.Factories.Pools
{
	public sealed class DamagePool : BasePool<EnemyDamagedArgs, Damage>
	{
		private readonly IFactory<Damage> _factory;
		private Transform _parrent;

		public DamagePool(IFactory<Damage> factory)
		{
			_factory = factory;
			Canvas canvas = UnityEngine.Object.FindObjectOfType<Canvas>();
			var parrent = new GameObject("Damage pool");
			parrent.transform.parent = canvas.transform;
			_parrent = parrent.transform;
		}

		protected override Damage Instantiate()
		{
			var system = _factory.Create();
			return system;
		}

		protected override async void OnSpawned(Damage entity, EnemyDamagedArgs param)
		{
			entity.transform.parent = _parrent;
			entity.transform.position = UnityEngine.Camera.main.WorldToScreenPoint(param.Position);
			entity.SetValue(param);
			entity.Start();
			await UniTask.Delay((int)(entity.LifeTime * 1000));
			Despawn(entity);
		}

		protected override void OnDespawned(Damage entity)
		{
		}
	}
}