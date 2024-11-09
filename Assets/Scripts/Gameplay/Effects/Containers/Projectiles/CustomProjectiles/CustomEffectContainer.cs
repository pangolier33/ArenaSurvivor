using Bones.Gameplay.Effects.Containers;
using Bones.Gameplay.Effects.Provider.Branches;
using Bones.Gameplay.Effects.Provider;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Railcar.Time;
using Bones.Gameplay.Utils;
using Bones.Gameplay.Stats.Containers.Graphs;
using Bones.Gameplay.Effects.Containers.Projectiles;
using Bones.Gameplay.Factories.Pools.Base;
using Bones.Gameplay.Players;
using Bones.Gameplay.Stats.Units;
using System.Threading;
using Bones.Gameplay.Entities;
using Bones.Gameplay.Effects.Provider.Variables;
using Bones.Gameplay.Factories.Pools;

namespace Bones
{
	[CreateAssetMenu(menuName = "Create Custom Effect", fileName = "Effect", order = 0)]
	public class CustomEffectContainer : ScriptableEffectContainer//TODO rm base class
	{
		[SerializeField] bool _useMulticast = true;
		[SerializeField] bool _useCount = true;
		[SerializeField] bool _spawnCountAsFan = false;
		[SerializeField] bool _searchEnemies = false;
		[SerializeField] float _multicastDelay = 0.1f;
		[SerializeField] ProjectileEffectContainer _prefab;

		[SerializeField] private AudioInfo _audioinfo;
		IPool<AudioInfo, AudioSource> _audioPool;
		float _audioSpawnTime;

		ITimer _timer;
		IMutableTrace _trace;
		CancellationTokenSource _subscription;
		EnemiesCollectionCache _enemies;
		Player _player;

		IPool<ProjectileSettings, ProjectileEffectContainer> _pool;

		public override IBranch Invoke(IMutableTrace trace)
		{
			if (trace is null)
				throw new ArgumentNullException(nameof(trace), $"Trace passed to {name} is null");
			var branch = new Branch($"Spell {name}");
			//_effect.Invoke(trace.Add(branch));
			_trace = trace;
			_player = trace.Get<Player>();
			_audioSpawnTime = 0.0f;

			_subscription = _timer.MarkAndRepeat(() => trace.GetStatValue<Gameplay.Stats.Units.Value>(StatName.Cooldown), delta => UpdateEffect(delta));

			return branch;
		}
		void UpdateEffect(float delta)
		{
			int count = 1;

			if (_useCount)
				count *= _trace.GetStatValue<Amount>(StatName.Count);

			if (_useMulticast)
				count *= _trace.GetStatValue<Amount>(StatName.Multicast);

			
			var enemies = _enemies.GetSorted(_trace.Get<ISpellActor>(), _trace.GetStatValue<Value>(StatName.Distance));
			var list = new List<Enemy>(enemies);

			if (list.Count == 0 || count == 0)
				return;
			
			for ( var i = 0; i < count; i++ )
			{
				Enemy e = list[i % list.Count];

				if (i == 0)
				{
					Spawn(delta, _trace.Add(e));

					if (_spawnCountAsFan)
					{
						int fanCount = _trace.GetStatValue<Amount>(StatName.Count);
						for (int offset = 0; offset < fanCount; ++offset)
						{
							Spawn(delta, _trace.Add(e).Add(new MutableVariable<int>("Offset", offset + 1)));
							Spawn(delta, _trace.Add(e).Add(new MutableVariable<int>("Offset", -offset - 1)));
						}
					}
				}
				else
				{
					var subscription = _timer.Mark(i * _multicastDelay, delta => Spawn(delta, _trace.Add(e)));

					if (_spawnCountAsFan)
					{
						int fanCount = _trace.GetStatValue<Amount>(StatName.Count);
						for (int offset = 0; offset < fanCount; ++offset)
						{
							int o1 = offset + 1;
							int o2 = -offset - 1;//Hack to capture by value
							var subscription2 = _timer.Mark(i * _multicastDelay, delta => Spawn(delta, _trace.Add(e).Add(new MutableVariable<int>("Offset", o1))));
							var subscription3 = _timer.Mark(i * _multicastDelay, delta => Spawn(delta, _trace.Add(e).Add(new MutableVariable<int>("Offset", o2))));
						}
					}
					//TODO manage subscription
				}
			}
		}

		void Spawn(float d, IMutableTrace trace)
		{
			var settings = new ProjectileSettings
			{
				Trace = trace,
			};
			var instance = _pool.Create(settings);

			if (_audioinfo.Clip && _audioSpawnTime < Time.time)
			{
				var source = _audioPool.Create(_audioinfo);
				source.transform.position = _player.Position;
				_audioSpawnTime = Time.time + _multicastDelay * 0.9f;
			}

			//TODO manage projectiles
		}

		[Inject]
		private void Inject(
			[Inject(Id = TimeID.Fixed)] ITimer timer, 
			IFactory<ProjectileEffectContainer, IPool<ProjectileSettings, ProjectileEffectContainer>> factory,
			EnemiesCollectionCache enemies, 
			IPool<AudioInfo, AudioSource> pool)
		{
			_timer = timer;
			_pool = factory.Create(_prefab);
			_enemies = enemies;
			_audioPool = pool;
		}
	}
}
