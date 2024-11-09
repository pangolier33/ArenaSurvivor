using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Events.Callbacks.Templates.Base;
using Bones.Gameplay.Spells.Classes;
using Bones.UI.Presenters.New;
using UniRx;
using UnityEngine;
using Zenject;

namespace Bones.Gameplay.Effects.Pure
{
	public sealed class RequestSpellSelectorPEC : PureEffect, IInjectable
	{
		[SerializeField] private BaseSpellContainer _spellContainer;

		private IMessagePublisher _publisher;
		
		protected override void Invoke(ITrace trace)
		{
			_publisher.Publish(new SpellSelectorRequestedArgs
			{
				Container = _spellContainer
			});
		}

		[Inject]
		private void Inject(DiContainer container, IMessagePublisher publisher)
		{
			_publisher = publisher;
			container.Inject(_spellContainer);
		}
	}
}