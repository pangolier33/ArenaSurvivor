using Bones.Analitics;
using Bones.Gameplay.Meta;
using Bones.Gameplay.Startup;
using UnityEngine;
using Zenject;

namespace Assets.Analitics
{
	public class AnaliticsSender : MonoBehaviour
	{
		private Level _level;
		private IWaveProvider _waveProvider;

		[Inject]
		private void Initialize(Level level, IWaveProvider waveProvider)
		{
			_level = level;
			_waveProvider = waveProvider;
			_level.Completed += OnLevelCompleted;
			_level.Failed += OnLevelFailed;
		}

		private void Start()
		{
			AppmetricaHelper.SendLevelStartEvent(_level.Current.Value);
		}

		private void OnLevelFailed()
		{
			AppmetricaHelper.SendLevelFailEvent(_level.Current.Value, (int)_waveProvider.TimeSpent);
		}

		private void OnLevelCompleted()
		{
			AppmetricaHelper.SendLevelCompleteEvent(_level.Current.Value - 1, (int)_waveProvider.TimeSpent);
		}
	}
}
