using System.Linq;
using Bones.Gameplay.Entities.Stats;
using Bones.Gameplay.Startup;
using Bones.Gameplay.Waves;
using Plugins.mitaywalle.ACV.Runtime;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using Zenject;

namespace Bones.UI.Presenters
{
	public class EnemyHealthFactorPresenter : MonoBehaviour, IInitializable
	{
		[SerializeField, Required] private TMP_Text label;
		[SerializeField, Required] private string format = "Enemies {0}";
		[Inject] private StatsBooster.Curves _curves;
		[Inject] private IWaveProvider _waveProvider;
		[Inject] private WaveConfig[] _waves;
		private TimeTrigger _timer;
		private float _allWavesDuration;

		public void Initialize()
		{
			_allWavesDuration = _waves.Where(x => x.Type != WaveType.Boss).Sum(x => x.Duration);
			_timer.Restart(5);
			Validate();
		}

		private void Update()
		{
			if (_timer.CheckAndRestart()) Validate();
		}

		private void Validate()
		{
			label.text = string.Format(format, _curves.HealthCurve.Evaluate(_waveProvider.GeneralTime));
		}
	}
}