using Bones.Gameplay.Effects.Provider.Branches;
using Bones.Gameplay.Effects.Provider.Debug;
using Bones.Gameplay.Players;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Bones.Gameplay.Effects.Containers
{
	public class MonoEffectContainer : MonoBehaviour, ITriggerEnterBridge, ITriggerExitBridge
	{
		[SerializeReference]
		[InlineProperty]
		[HideLabel]
		private IEffect _effect;

		private IBranch _branch;

		public void OnTriggered(GameObject gameObject)
		{
			if (!gameObject.TryGetComponent(out Player component))
				return;
			_effect.Invoke(DebugNode.Root.Add(_branch = new Branch($"Entity {gameObject.name}")).Add(component));
		}

		public void OnOut(GameObject gameObject)
		{
			if (!gameObject.TryGetComponent(out Player _))
				return;
			_branch.Dispose();
		}
		[Inject]
		private void Inject(DiContainer container)
		{
			container.Inject(_effect);
		}
	}
}