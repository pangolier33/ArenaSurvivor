using System;
using System.Collections.Generic;
using Bones.Gameplay.Entities;
using Bones.Gameplay.Events.Args;
using Sirenix.OdinInspector;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace Bones.UI.Presenters
{
	public class EnemyCountPresenter : MonoBehaviour, IInitializable, IDisposable
	{
		[SerializeField, Required] private TMP_Text _label;
		[SerializeField, Required] private string _format = "Enemies {0}";

		private CompositeDisposable _brokerSubscriptions = new();
		private HashSet<Enemy> _enemies = new();
		private IMessageReceiver _receiver;

		void IInitializable.Initialize()
		{
			_receiver.Receive<EnemySpawnedArgs>()
				.Select(x => x.Subject)
				.Subscribe(OnSpawned)
				.AddTo(_brokerSubscriptions);

			_receiver.Receive<EnemyDiedArgs>()
				.Select(x => x.Subject)
				.Subscribe(OnDespawned)
				.AddTo(_brokerSubscriptions);
		}

		[Inject]
		private void Inject(IMessageReceiver receiver) => _receiver = receiver;

		private void OnSpawned(Enemy enemy)
		{
			if (!_enemies.Contains(enemy))
			{
				_enemies.Add(enemy);
				Validate();
			}
		}

		private void OnDespawned(Enemy enemy)
		{
			if (_enemies.Contains(enemy))
			{
				_enemies.Remove(enemy);
			}
			Validate();
		}

		private void Validate()
		{
			_label.text = string.Format(_format, _enemies.Count);
		}

		void IDisposable.Dispose()
		{
			_brokerSubscriptions?.Dispose();
			_enemies.Clear();
		}
	}
}