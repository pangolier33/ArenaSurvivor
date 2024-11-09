using Bones.Gameplay.Effects.Transitive.Depth;
using Bones.Gameplay.Entities;
using Bones.Gameplay.Stats.Containers.Graphs;
using Bones.Gameplay.Stats.Units;
using System;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

namespace Bones.UI.Views
{
	public class HealthView : MonoBehaviour
	{
		[SerializeField] private Enemy _enemy;
		[SerializeField] private Image _image;

		private IDisposable _healthObserver;
		private Value _maxHealth;

		private void Awake()
		{
			_enemy.Spawned += OnEnemySpawned;
		}

		private void OnDestroy()
		{
			_enemy.Spawned -= OnEnemySpawned;
		}

		private void OnEnemySpawned()
		{
			var healthReadOnlyStat = (ReadOnlyStat<Value>)_enemy.Stats.Get(StatName.OverallHealth);
			_healthObserver = healthReadOnlyStat.Subscribe(OnHealthUpdated);
			_maxHealth = healthReadOnlyStat.BaseValue;
		}

		private void OnDisable()
		{
			_healthObserver?.Dispose();
		}

		private void OnHealthUpdated(Value value)
		{
			_image.fillAmount = Mathf.Clamp01((float)(value.Base / _maxHealth.Base));
		}
	}
}
