using System;
using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Effects.Provider.Debug;
using Bones.Gameplay.Events.Callbacks.Templates.Base;
using Bones.Gameplay.Players;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Bones.Gameplay.Spells.Classes
{
	[CreateAssetMenu]
	public class SpellsInstaller : ScriptableObjectInstaller
	{
		[SerializeField]
		[InlineProperty]
		[HideLabel]
		private SerializableCustomStatsConfiguration _statsDeclaration;

		[SerializeField]
		[InlineProperty]
		[HideLabel]
		private BaseSpellContainer _playerContainer;
		
		public override void InstallBindings()
		{
			if (_playerContainer == null)
				throw new NullReferenceException("Set the player spells container in spells installer");

			Container.QueueForInject(_statsDeclaration);
			Container.Bind<IMutableTrace>().FromMethod(context => DebugNode.Root
				.Add(context.Container.Resolve<Player>())
				.Add(context.Container.Resolve<IStatMap>())
				.Add(_statsDeclaration.Create()))
				.AsCached()
				.Lazy();
			
			if (_playerContainer is IInitializable initializablePlayerController)
				Container.BindInstance(initializablePlayerController);
			if (_playerContainer is IInjectable)
				Container.QueueForInject(_playerContainer);

			Container.BindInstance<ISpellContainer>(_playerContainer);
		}
	}
}