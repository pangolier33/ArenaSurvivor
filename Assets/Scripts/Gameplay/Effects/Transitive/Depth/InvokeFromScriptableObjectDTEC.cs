using System;
using Bones.Gameplay.Effects.Containers;
using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Events.Callbacks.Templates.Base;
using UnityEngine;
using Zenject;

namespace Bones.Gameplay.Effects.Transitive.Depth
{
	[Serializable]
	public class InvokeFromScriptableObjectDTEC : IEffect, IInjectable
	{
		[SerializeReference] private ScriptableEffectContainer _effect;
		
		public void Invoke(IMutableTrace trace)
		{
			trace.ConnectToClosest(_effect.Invoke(trace));
		}

		[Inject]
		private void Inject(DiContainer container)
		{
			container.Inject(_effect);
		}
	}
}