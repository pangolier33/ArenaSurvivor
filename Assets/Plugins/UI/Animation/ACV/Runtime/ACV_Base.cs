using System;
using UnityEngine;
using UnityEditor;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine.Events;

namespace Plugins.mitaywalle.ACV.Runtime
{
	public abstract class ACV_Base : MonoBehaviour
	{
		[HideLabel, ShowInInspector,PropertyRange("_minValue","_maxValue")]
		private float InspectorValue
		{
			get => _value;
			set => Set(value);
		}

		[SerializeField] protected AnimationTimePattern _pattern;
		[SerializeField, OnValueChanged("SetValue")] protected float _value;
		[SerializeField, OnValueChanged("SetValue")] protected float _minValue;
		[SerializeField, OnValueChanged("SetValue")] protected float _maxValue = 100;
		[SerializeField] protected float _speed = 1;
		[SerializeField] protected float _fixedTime = .35f;
		[SerializeField] protected float _defaultAlpha = 1;
		[SerializeField] protected float _flashAlphaTime = 1.5f;
		[SerializeField, Range(0, 1)] protected float _alphaOnSet = 1;

		[SerializeField] protected bool _isDebugging;
		[SerializeField] protected bool _ifChangeOnly;
		[SerializeField] protected bool _disableOnFinish;
		[SerializeField] protected bool _hideIfZeroGameObject;
		[SerializeField] protected bool _hideIfZeroCanvasGroup;
		[SerializeField] protected bool _useSound;

		[SerializeField, Header("References")] protected GameObject _objectToHide;
		[SerializeField] protected CanvasGroup _canvasGroupToHide;
		[SerializeField] protected CanvasGroup _canvasGroup;
		[SerializeField] protected AudioSource _audioSource;

		protected TimeTrigger _flashTrigger = new TimeTrigger(2);
		protected Coroutine _animatingCoroutine;
		protected bool _isAnimating;
		protected float _animatedValue;
		protected float _lastMax;
		protected float _lastValue;

		public float GetMinValue() => _minValue;
		public float GetMaxValue() => _maxValue;

		public UnityEvent<float, float> OnSet;

		public bool GetIsAnimating() => _isAnimating;

		public void SetMinValue(float value) => _minValue = value;
		public void SetMaxValue(float value) => _maxValue = value;

		public void FlashAlpha()
		{
			_flashTrigger.Restart(_flashAlphaTime);
		}

		protected virtual void OnEnable()
		{
			if (_canvasGroup) _canvasGroup.alpha = _defaultAlpha;
		}

		internal void Animate(float deltaTime)
		{
			float speed = 1;

			switch (_pattern)
			{
				case AnimationTimePattern.Speed:
					speed = _speed;

					break;

				case AnimationTimePattern.FixedTime:
					speed = Mathf.Abs((_value - _lastValue) / _fixedTime);

					break;

				default:
					throw new ArgumentOutOfRangeException();
			}

			_animatedValue = Mathf.MoveTowards(_animatedValue, _value, deltaTime * speed);

			if (_isDebugging) Debug.LogError($"Animate animatedValue = {_animatedValue} , value = {_value}");

			SetValue();
		}

		protected void PlaySound()
		{
			if (_audioSource) _audioSource.Play();
		}

		public void SetBackValue(float newValue)
		{
			_value = newValue;
		}

		public virtual void SetValue()
		{
			if (_isDebugging) Debug.LogError($"SetValue {_value}");

			if (!_ifChangeOnly || _value != _lastValue || _lastMax != _maxValue)
			{
				if (_hideIfZeroGameObject && _objectToHide)
				{
					_objectToHide.SetActive(_value != 0);
				}

				if (_hideIfZeroCanvasGroup && _canvasGroupToHide)
				{
					_canvasGroupToHide.Show(_value != 0);
				}

				OnSet?.Invoke(_value, _maxValue);
			}
		}

		public float GetValue()
		{
			return _value;
		}

		public virtual void Set(float fromValue, float newValue, bool animated = true)
		{
			_lastValue = fromValue;

			_animatedValue = fromValue;
			_value = newValue;

			SetInternal(newValue, animated);
		}

		[Button]
		public void Set(float newValue) => Set(newValue, true);
		public virtual void Set(float newValue, bool animated)
		{
			_lastValue = _value;

			_animatedValue = _value;
			_value = newValue;

			SetInternal(newValue, animated);
		}
		protected virtual void SetInternal(float newValue, bool animated = true)
		{
			if (animated && _useSound) PlaySound();

			bool setAlpha = _canvasGroup && _defaultAlpha != _alphaOnSet;
			if (setAlpha && _canvasGroup) _canvasGroup.alpha = _defaultAlpha;

			if (_isDebugging) Debug.LogError($"Value = {newValue} / {_maxValue}", this);

			if (_isAnimating)
			{
				if (_animatingCoroutine != null) StopCoroutine(_animatingCoroutine);
				_isAnimating = false;
			}

			SetValue();

			if (animated && gameObject.activeInHierarchy)
			{
				_animatingCoroutine = StartCoroutine(Animating());
			}
			else
			{
				_animatedValue = _value;
				SetValue();
			}
		}

		IEnumerator Animating()
		{
			_isAnimating = true;

			if (_isDebugging) Debug.LogError($"Animating Coroutine value = {_value}");

			bool setAlpha = _canvasGroup && _defaultAlpha != _alphaOnSet;

			if (setAlpha && _canvasGroup) _canvasGroup.alpha = _alphaOnSet;
			FlashAlpha();

			while (_animatedValue != _value)
			{
				Animate(Time.deltaTime);

				yield return null;
			}

			while (!_flashTrigger.IsReady()) yield return null;

			if (setAlpha && _canvasGroup) _canvasGroup.alpha = _defaultAlpha;

			if (_isDebugging) Debug.LogError($"value = {_value}");

			if (_disableOnFinish) DisableTarget();
			_isAnimating = false;
		}

		public abstract void DisableTarget();

		[Button("Reset Layout")]
		public virtual void ResetLayout()
		{
			ResetLayout(true);
		}

		public virtual void ResetLayout(bool setValue)
		{
		}

		[Button(nameof(Reset))]
		protected virtual void Reset()
		{
#if UNITY_EDITOR
			Undo.RecordObject(this, "fill refs AnimatedChangeValueUI");

			if (_canvasGroup) _canvasGroup.alpha = _defaultAlpha;
#endif
		}
	}
}