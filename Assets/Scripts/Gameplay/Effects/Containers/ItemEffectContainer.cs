using System;
using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Effects.Provider.Branches;
using Bones.Gameplay.Items;
using Bones.Gameplay.Stats.Containers.Builders.Wrappers;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Bones.Gameplay.Effects.Containers
{
	public sealed class ItemEffectContainer : MonoBehaviour
	{
		[SerializeReference]
		private SerializableWrappedStatsConfiguration _statsConfiguration;
		
		[SerializeReference]
		[InlineProperty]
		[HideLabel]
		private IEffect _effect;

		private IMutableTrace _trace;
		
		private void Awake()
		{
			GetComponent<Item>().Disposed += OnDisposed;
			if (_statsConfiguration != null)
				_trace = _trace.Add(_statsConfiguration.Create());
		}
        
		private void OnDisposed(object sender, EventArgs e)
		{
			_effect.Invoke(_trace.Add(new Branch()));
		}
        
		[Inject]
		private void Inject(DiContainer container, IMutableTrace trace)
		{
			_trace = trace;
			container.Inject(_effect);
			if (_statsConfiguration != null)
				container.Inject(_statsConfiguration);
		}
	}
}