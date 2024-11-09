using System;
using Bones.Gameplay.Experience;
using Bones.Gameplay.Spells.Classes;
using Bones.UI.Presenters.New;
using UniRx;
using Zenject;

namespace Bones.Gameplay.Startup
{
	public class LevelUpHandler : IInitializable, IDisposable
	{
		private IExperienceBank _bank;
		private IMessagePublisher _publisher;
		private ISpellContainer _container;

		private IDisposable _subscription;
		
		public void Initialize()
		{
			_subscription = _bank.Level.Subscribe(OnLevelUpped);
		}

		public void Dispose()
		{
			_subscription.Dispose();
		}
		
		private void OnLevelUpped(int value)
		{
			_publisher.Publish(new SpellSelectorRequestedArgs { Container = _container, Level = value });
		}

		[Inject]
		private void Inject(IExperienceBank bank, IMessagePublisher publisher, ISpellContainer container)
		{
			_bank = bank;
			_publisher = publisher;
			_container = container;
		}
	}
}