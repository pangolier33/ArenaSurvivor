using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Events.Callbacks.Templates.Base;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Bones.Gameplay.Players
{
	[CreateAssetMenu]
	public class ScriptablePlayerStatsConfiguration : ScriptableObject, IFactory<IStatMap>, IInjectable
	{
		[SerializeField]
		[HideLabel]
		[InlineProperty]
		private SerializablePlayerStatsConfiguration _statsConfiguration;
		
		public IStatMap Create() => _statsConfiguration.Create();
		
		[Inject]
		private void Inject(DiContainer container)
		{
			container.Inject(_statsConfiguration);
		}
	}
}