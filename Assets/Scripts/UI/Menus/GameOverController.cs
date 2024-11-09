using Bones.Analitics;
using Bones.Gameplay.Events.Args;
using Bones.Gameplay.Meta;
using Bones.Gameplay.Meta.Currencies;
using Bones.Gameplay.Startup;
using Railcar.Time;
using UniRx;
using UnityEngine;
using Zenject;

namespace Bones.UI.Menus
{
	public class GameOverController : MonoBehaviour, IInitializable
	{
		[SerializeField] private GameOverMenu _screen;
		[SerializeField] private GameObject _containerScreen;

		private IMessageReceiver _receiver;
		private DiContainer _container;
		private ITimeLord _timeLord;
		private Level _level;
		private bool _isShown = false;

		public void Initialize() => _receiver.Receive<PlayerDiedArgs>().Subscribe(SpawnScreen);

		private void SpawnScreen(PlayerDiedArgs player)
		{
			if (_isShown)
				return;
			_timeLord.DoesFlow = false;
			var screen = Instantiate(_screen, _containerScreen.transform);
			_container.Inject(screen);
			screen.Open();
			_isShown = true;
			_level.FailLevel();
		}

		[Inject]
		private void Inject(IMessageReceiver receiver, 
			DiContainer container, 
			ITimeLord timeLord,
			Level level)
		{
			_receiver = receiver;
			_container = container;
			_timeLord = timeLord;
			_level = level;
		}
	}
}
