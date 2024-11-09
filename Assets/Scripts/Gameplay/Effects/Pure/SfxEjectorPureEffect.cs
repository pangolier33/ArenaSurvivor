using System;
using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Effects.Transitive.Depth;
using Bones.Gameplay.Events.Callbacks.Templates.Base;
using Bones.Gameplay.Factories.Pools;
using Bones.Gameplay.Factories.Pools.Base;
using UnityEngine;
using Zenject;

namespace Bones.Gameplay.Effects.Pure
{
	[Serializable]
	public class SfxEjectorPureEffect : PureEffect, IInjectable
	{
		[SerializeField] private AudioInfo _info;

		private IPool<AudioInfo, AudioSource> _pool;

		protected override void Invoke(ITrace trace)
		{
			var source = _pool.Create(_info);
			
			var entity = trace.TryGet<IPositionSource>();
			if (entity != null)
				source.transform.position = entity.Position;
		}

		[Inject]
		private void Inject(IPool<AudioInfo, AudioSource> pool)
		{
			_pool = pool;
		}
	}
}