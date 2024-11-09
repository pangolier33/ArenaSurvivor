using System;
using Bones.Gameplay.Effects.Containers.Projectiles;
using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Events.Callbacks.Templates.Base;
using Bones.Gameplay.Factories.Pools.Base;
using Bones.Gameplay.Players;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Bones.Gameplay.Effects.Transitive.Width
{
	[Serializable]
	public class ProjectileSpawningEffect : IEffect, IInjectable
	{
		[SerializeField] private ProjectileEffectContainer _prefab;
		[SerializeReference, HideReferenceObjectPicker] private IEffect _triggeredEffect = default!;
		[SerializeReference, HideReferenceObjectPicker] private IEffect _spawnedEffect = default!;

		private IPool<ProjectileSettings, ProjectileEffectContainer> _pool;

		public void Invoke(IMutableTrace trace)
		{
			var settings = new ProjectileSettings
			{
				Trace = trace,
				OnTriggeredEffect = _triggeredEffect,
				OnSpawnedEffect = _spawnedEffect
			};
			var instance = _pool.Create(settings);
		}

		[Inject]
		private void Inject(DiContainer container, Player player, IFactory<ProjectileEffectContainer, IPool<ProjectileSettings, ProjectileEffectContainer>> factory)
		{
			_pool = factory.Create(_prefab);
			container.Inject(_triggeredEffect);
			container.Inject(_spawnedEffect);
		}
	}
}