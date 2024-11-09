using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Plugins.Animated.Pulsating
{
	public abstract class PulsatingBase : MonoBehaviour
	{
		public enum eType
		{
			Sin = 0,
			Tangent = 1,
			FromTo = 2,
			Perlin = 3,
		}

		[ShowInInspector] protected bool debugging;

		public eType myType;

		[SerializeField] protected float speed = 5;
		[SerializeField] protected float value;
		[SerializeField] protected float min;
		[SerializeField] protected float max = 1;
		[SerializeField] protected float stopValue = 1;
		[SerializeField] protected bool enableOnEnable = true;

		public abstract void SetValue(float value);
		public abstract void Animate(float time, float deltaTime);
		public abstract void Enable(bool state = true, float startValue = 0f, float disableTimer = float.MaxValue, float finishValue = 1f);
		public abstract void Disable(float stayValue = 0f);
		public abstract void ValueApply();
		[Button]
		public abstract void Toggle();
		[Button]
		public abstract void Reset();
	}

	public abstract class PulsatingUI<T> : PulsatingBase where T : Component
	{
		public T target;

		private Coroutine _animatingCoroutine;

		private void OnEnable()
		{
			if (enableOnEnable) Enable();
		}

		public override void SetValue(float value)
		{
			base.value = value;
			ValueApply();
		}

		public override void Animate(float time, float deltaTime)
		{
			switch (myType)
			{
				case eType.Sin:
					value = Mathf.Lerp(min, max, (1 + Mathf.Sin(time * speed)) * .5f);
					break;
				case eType.Tangent:
					value = Mathf.Lerp(min, max, (1 + Mathf.Tan(time * speed)) * .5f);
					break;
				case eType.FromTo:
					value = Mathf.MoveTowards(value, max, speed * deltaTime);
					break;
				case eType.Perlin:
					value = Mathf.LerpUnclamped(min, max, (Mathf.PerlinNoise(time * speed, 0f) - .5f) * 2f);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			if (debugging) Debug.LogError($"set value = {value}");

			ValueApply();
		}

		public override void Enable(bool state = true, float startValue = 0f, float disableTimer = float.MaxValue, float finishValue = 1f)
		{
			value = startValue;
			enabled = state;

			if (state && gameObject.activeInHierarchy)
			{
				if (_animatingCoroutine != null)
				{
					StopCoroutine(_animatingCoroutine);
				}
				_animatingCoroutine = StartCoroutine(Animating(startValue, disableTimer, finishValue));
			}
			else
			{
				if (_animatingCoroutine != null) StopCoroutine(_animatingCoroutine);
			}

			ValueApply();
		}

		private IEnumerator Animating(float startValue = 0f, float disableTimer = float.MaxValue, float finishValue = 1f)
		{
			SetValue(startValue);
			var time = Mathf.Asin(startValue);

			var check = true;

			switch (myType)
			{
				case eType.Sin:
				case eType.Tangent:
				case eType.Perlin:
					break;
				case eType.FromTo:
					finishValue = max;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			while (check)
			{
				switch (myType)
				{
					case eType.Sin:
					case eType.Tangent:
					case eType.Perlin:
						check = time < disableTimer;
						break;
					case eType.FromTo:
						check = Math.Abs(value - finishValue) > .01f;
						break;

					default:
						throw new ArgumentOutOfRangeException();
				}
				Animate(time += Time.deltaTime, Time.deltaTime);

				yield return null;
			}

			while (Math.Abs(value - finishValue) > .01f)
			{
				SetValue(Mathf.MoveTowards(value, finishValue, speed * Time.deltaTime));
				yield return null;
			}
		}

		public override void Disable(float stayValue = 0f)
		{
			Enable(false, stayValue);
		}

		public override void Toggle()
		{
			Enable(!enabled);
		}

		public override void ValueApply()
		{
			if (debugging) Debug.Log($"set value = {value}");
		}

		public override void Reset()
		{
#if UNITY_EDITOR
			Undo.RecordObject(this, "fill refs PulsatingUI");
#endif
			if (!target) target = GetComponent<T>();
#if UNITY_EDITOR
			EditorUtility.SetDirty(this);
#endif
		}
	}
}