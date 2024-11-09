using System;
using System.Linq;
using Bones.Gameplay.Events.Args;
using Bones.Gameplay.LevelLoader;
using Bones.Gameplay.Meta;
using Bones.Gameplay.Meta.Currencies;
using Bones.Gameplay.Waves;
using Railcar.Time;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;


namespace Bones.Gameplay.Startup
{
	[Serializable]
	public class WavesHandler : IDisposable, IWaveProvider
	{
		private CompositeDisposable _spawningSubscriptions = new();
		private IDisposable _timeSubscription;
		private ICurrencyStorage _currenciesStorage;
		private Level _level;
		private SpawnerConfig.Adapter _adapter;
		private PlaceholderFactory<DraftSpawner.Settings, DraftSpawner> _spawnerFactory;

		[ShowInInspector] private ReactiveProperty<float> _waveDuration = new();
		[ShowInInspector] private ReactiveProperty<int> _index = new();
		private ReactiveProperty<WaveType> _type = new();
		private ReactiveProperty<float> _generalDuration = new();

		[ShowInInspector] private WaveConfig[] _waveConfigs;

		[Inject]
		private void Inject(
			[Inject(Id = TimeID.Fixed)] IStopwatch stopwatch,
			[Inject] SpawnerConfig.Adapter adapter,
			[Inject] PlaceholderFactory<DraftSpawner.Settings, DraftSpawner> spawnerFactory,
			[Inject] WaveConfig[] waveConfigs,
			[Inject] IMessageReceiver receiver,
			Level level,
			ICurrencyStorage currenciesStorage
			)
		{
			_currenciesStorage = currenciesStorage;
			_level = level;
			_adapter = adapter;
			_spawnerFactory = spawnerFactory;
			_waveConfigs = waveConfigs;
			receiver.Receive<BossDiedArgs>().Where(x => !x.MiniBoss).Subscribe(_ => Iterate());

			_index.Value = -1;
			Iterate();
			_timeSubscription = stopwatch.Observe(OnUpdated);
		}

		IObservable<WaveConfig> IWaveProvider.Wave => _index.Where(x => x > -1 && x < _waveConfigs.Length).Select(x => _waveConfigs[x]);
		public IObservable<int> Index => _index;
		public IObservable<float> Completeness => _waveDuration.Select(x => x / Wave.Duration);
		public float GeneralTime => _generalDuration.Value;
		private WaveConfig Wave => _waveConfigs[_index.Value];

		public float TimeSpent => _waveDuration.Value + _waveConfigs.Take(_index.Value).Sum(x => x.Duration);

		private void OnUpdated(float delta)
		{
			_waveDuration.Value += delta;
			if (Wave.Type != WaveType.Boss)
				_generalDuration.Value += delta;
			if (_waveDuration.Value > Wave.Duration)
				Iterate();
		}

		public void Dispose()
		{
			_spawningSubscriptions.Dispose();
			_timeSubscription.Dispose();
		}

		[Button]
		private void Iterate()
		{
			_spawningSubscriptions.Clear();
			_waveDuration.Value = 0;

			if (_index.Value + 1 >= _waveConfigs.Length)
			{
				_level.CompleteLevel();
				Debug.Log("You Win! level finished");
				return;
			}
			_index.Value++;

			var wave = _waveConfigs[_index.Value];
			foreach (var spawnerConfig in wave.Spawners)
			{
				var spawnerSettings = _adapter.Translate(spawnerConfig);
				_spawningSubscriptions.Add(_spawnerFactory.Create(spawnerSettings));
			}
		}
	}
}