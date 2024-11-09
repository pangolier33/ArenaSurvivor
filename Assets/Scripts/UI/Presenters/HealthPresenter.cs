using System;
using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Players;
using Bones.Gameplay.Stats.Containers;
using Bones.Gameplay.Stats.Containers.Graphs;
using Bones.Gameplay.Stats.Units;
using Bones.Gameplay.Stats.Utils;
using Bones.Gameplay.Utils;
using Bones.UI.Views;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UniRx;
using TweenExtensions = Bones.Gameplay.Utils.TweenExtensions;
namespace Bones.UI.Presenters
{
    public class HealthPresenter : MonoBehaviour, IInitializable, IDisposable
    {
        [Header("Bars")]
        [SerializeField] private RadialBarView _healthView;
        [SerializeField] private RadialBarView _shieldView;
		[SerializeField] private Image _staminaView;
		[SerializeField] private AnimationCurve _sizeByValue;

        [Header("Background")]
        [SerializeField] private Image _background;
        [SerializeField] private Gradient _backgroundGradient;
        [SerializeField] private float _backgroundColoringDuration;

        private readonly CompositeDisposable _subscriptions = new();
        
        private IStatMap _stats;

        private Value _health;
        private Value _maxHealth;
        private Value _shield;
        private Value _maxShield;
		private Value _stamina;
		private Value _maxStamina;

		public void Initialize()
        {
            _stats.Get(StatName.Health).ObserveGetAndSet<Value>().Subscribe(SetHealth).AddTo(_subscriptions);
            _stats.Get(StatName.MaxHealth).ObserveEveryValueChanged(x => ((IGetStat<Value>)x).Get()).Subscribe(SetMaxHealth).AddTo(_subscriptions);
            _stats.Get(StatName.Shield).ObserveGetAndSet<Value>().Subscribe(SetShield).AddTo(_subscriptions);
            _stats.Get(StatName.MaxShield).ObserveEveryValueChanged(x => ((IGetStat<Value>)x).Get()).Subscribe(SetMaxShield).AddTo(_subscriptions);
			_stats.Get(StatName.Stamina).ObserveGetAndSet<Value>().Subscribe(SetStamina).AddTo(_subscriptions);
			_maxStamina = new Value(((IGetStat<StaminaValue>)_stats.Get(StatName.StaminaParametrs)).Get().maxScore);
		}
        
        public void Dispose()
        {
            _subscriptions.Dispose();
        }
        
        private void SetHealth(Value value)
        {
            if (_health == value)
                return;
            
            _health = value;
            UpdateHealthBarValue();
            UpdateFade();
        }

        private void SetMaxHealth(Value value)
        {
            if (_maxHealth == value)
                return;

            _maxHealth = value;
            UpdateHealthBarValue();
            UpdateHealthBarMax();
            UpdateFade();
        }

        private void SetShield(Value value)
        {
            if (_shield == value)
                return;

            _shield = value;
            UpdateShieldBarValue();
        }

        private void SetMaxShield(Value value)
        {
            if (_maxShield == value)
                return;

            _maxShield = value;
            UpdateShieldBarValue();
            UpdateShieldBarMax();
        }

		private void SetStamina(Value value)
		{
			if (_stamina == value)
				return;

			_stamina = value;
			UpdateStaminaBarValue();
		}

		private void UpdateFade()
        {
            var value = _maxHealth == 0 ? 0 : _health / _maxHealth;
            var color = _backgroundGradient.Evaluate(value);
            _background.DOColor(color, _backgroundColoringDuration);
        }

        private void UpdateHealthBarValue() => UpdateBarValue(_healthView, _health);
        private void UpdateShieldBarValue() => UpdateBarValue(_shieldView, _shield);

		private void UpdateStaminaBarValue() => _staminaView.fillAmount = _stamina / _maxStamina;
		private void UpdateHealthBarMax() => UpdateBarMax(_healthView, _maxHealth);
        private void UpdateShieldBarMax() => UpdateBarMax(_shieldView, _maxShield);

		private void UpdateBarValue(RadialBarView bar, Value value) => bar.UpdateValue(_sizeByValue.Evaluate(value));
        private void UpdateBarMax(RadialBarView bar, Value value) => bar.SetMax(_sizeByValue.Evaluate(value));
        
        [Inject]
        private void Inject(Player player)
        {
            _stats = player.Stats;
        }
    }
}