using System;
using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Effects.Provider.Branches;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Bones.Gameplay.Effects.Containers
{
	[CreateAssetMenu(menuName = "Create Effect", fileName = "Effect", order = 0)]
	public class ScriptableEffectContainer : ScriptableObject
	{
		[SerializeReference, HideLabel, InlineProperty]
		private IEffect _effect;

		public virtual IBranch Invoke(IMutableTrace trace)
		{
			if (trace is null)
				throw new ArgumentNullException(nameof(trace), $"Trace passed to {name} is null");
			var branch = new Branch($"Spell {name}");
			_effect.Invoke(trace.Add(branch));
			return branch;
		}

		[Inject]
		private void Inject(DiContainer container)
		{
			if (_effect == null)
				throw new NullReferenceException($"Effect is not set inside container {name}");
			container.Inject(_effect);
		}
	}
}