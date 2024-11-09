using System;
using System.Collections;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Plugins.Own.Animated
{
	public abstract class Animated : MonoBehaviour
	{
		protected const bool DebuggingGlobal = true;
		protected static readonly Vector3 MIN_SCALE = Vector3.one * 0.0001f;

		[Title("Data"), InlineProperty, HideLabel] public AnimationData data;
		public Animated[] child;
		public Animated[] next;

		[SerializeField] protected AudioSource AS;
		[SerializeField] protected GameObject OnPlay;

		public float timedTime;

		[SerializeField] protected bool CanContinue;
		[SerializeField] protected bool Debugging;
		public bool ResetOnEnable = true;
		public bool PlayOnEnable = true;
		public bool DisableOnEnd;
		public bool StopSoundOnEnd;
		public bool Loop;

		protected float RandomCurveLerpFactor;
		protected float startTime;
		protected float t;

		public event Action OnTime;
		public event Action OnReset;
		public event Action OnPlayAction;
		public event Action OnFinishAction;
		public Action OnFinish;

		public bool HasFinishAction => OnFinish != null;

		internal bool Animating;

		protected Coroutine cor;

		[Serializable]
		public class AnimationData
		{
			public AnimationType type;

			public float MaxValue = 1;
			public float Delay;
			public float Period = .3f;
			public float PostDelay;
			public float Speed = 1;
			public bool invert;
			[ShowIf("@type.HasFlag(AnimationType.Scale)")] public bool uniformScale;
			[ShowIf("@type.HasFlag(AnimationType.Scale)")] public bool clampScale;

			[ShowIf("@type.HasFlag(AnimationType.Fill)")] public ParticleSystem.MinMaxCurve Fill;

			[ShowIf("@type.HasFlag(AnimationType.Color)")] public ParticleSystem.MinMaxGradient Color = new(UnityEngine.Color.white, UnityEngine.Color.white);

			[ShowIf("@type.HasFlag(AnimationType.Position)"), BoxGroup("p", showLabel: false), InlineProperty] public Vector3MinMax Position;
			[ShowIf("@type.HasFlag(AnimationType.Rotation)"), BoxGroup("r", showLabel: false), InlineProperty] public Vector3MinMax Rotation;
			[ShowIf("@type.HasFlag(AnimationType.Scale)"), InlineProperty] public Vector3MinMax Scale;

			[ShowIf("@type.HasFlag(AnimationType.Rect) || type.HasFlag(AnimationType.RectSize)")]
			public ParticleSystem.MinMaxCurve Rect;
			[ShowIf("@type.HasFlag(AnimationType.Rect) || type.HasFlag(AnimationType.RectSize)")]
			public RectTransform from;
			[ShowIf("@type.HasFlag(AnimationType.Rect) || type.HasFlag(AnimationType.RectSize)")]
			public RectTransform to;
		}

		protected bool IsFinished()
		{
			return (!(t < 1f) || data.invert) && (!data.invert || !(t > 0f));
		}

        #region ResetValue
		public virtual void ResetValue(float normalizedTime = 0f)
		{
#if UNITY_EDITOR
			if (!Application.isPlaying) GetComponents<AnimatedEvents>().ToList().ForEach(t => t.Init());
#endif

			Stop();

			if (OnPlay != null) OnPlay.SetActive(normalizedTime == 1f);

			t = data.invert ? 1f - normalizedTime : normalizedTime;
#if UNITY_EDITOR
			if (Debugging) Debug.LogError($"reset {normalizedTime} {t}", gameObject);
#endif
			SetValue(t);

			OnReset?.Invoke();

			if (child != null)
				for (int i = 0; i < child.Length; i++)
					child[i].ResetValue(normalizedTime);
			if (next != null)
				for (int i = 0; i < next.Length; i++)
					next[i].ResetValue(normalizedTime);
		}

		[Button("Reset Value 0")]
		public void ResetValue0()
		{
			ResetValue(0f);
		}

		[Button("Reset Value 0.5")]
		public void ResetValue05()
		{
			ResetValue(.5f);
		}

		[Button("Reset Value 1")]
		public void ResetValue1()
		{
			ResetValue(1f);
		}

		[Button("Set Value 0")]
		public void SetValue0()
		{
			SetValue(0f);
		}

		[Button("Set Value 0.5")]
		public void SetValue05()
		{
			SetValue(.5f);
		}

		[Button("Set Value 1")]
		public void SetValue1()
		{
			SetValue(1f);
		}
        #endregion

		protected virtual void OnEnable()
		{
			if (data.type == 0) return;

			if (ResetOnEnable) ResetValue();

			if (!PlayOnEnable) return;

			Play();
		}

		public void Play(bool invert)
		{
			data.invert = invert;
			Play();
		}

		[Button("Play")]
		public virtual void PlaySafe(bool invert = false)
		{
			if (gameObject.activeInHierarchy)
			{
				Play(invert);
				return;
			}
			data.invert = invert;
			ResetValue1();
		}

		public virtual bool Play()
		{
			if (!enabled)
			{
#if UNITY_EDITOR
				if (Debugging && DebuggingGlobal) Debug.Log("Play fail", gameObject);
#endif
				return false;
			}

			if (!gameObject.activeInHierarchy)
			{
#if UNITY_EDITOR
				if (Debugging && DebuggingGlobal) Debug.Log("Play fail", gameObject);
#endif
				return false;
			}

			//if (!gameObject.activeSelf) gameObject.SetActive(true);

#if UNITY_EDITOR
			if (Debugging && DebuggingGlobal) Debug.Log($"Play 0 {name}", gameObject);
#endif

			Animating = false;

			if (!gameObject.activeSelf) gameObject.SetActive(true);

			if (!CanContinue) ResetValue();

			OnPlayAction?.Invoke();
			if (OnPlay != null) OnPlay.SetActive(true);

			if (child != null)
				for (int i = 0; i < child.Length; i++)
					child[i].Play();

#if UNITY_EDITOR
			if (Debugging && DebuggingGlobal) Debug.LogError("Play 1", gameObject);
#endif

			cor = StartCoroutine(Animate());
			return true;
		}

		[Button("Stop")]
		public virtual void Stop()
		{
#if UNITY_EDITOR
			if (Debugging && DebuggingGlobal) Debug.LogError($"Stop 0 {name}", gameObject);
#endif
			if (child != null)
				for (int i = 0; i < child.Length; i++)
					child[i].Stop();
			if (next != null)
				for (int i = 0; i < next.Length; i++)
					next[i].Stop();

			OnStop();

			if (cor != null)
			{
				StopCoroutine(cor);
				cor = null;
			}

			Animating = false;
		}

		protected abstract void BeforePlay();
		protected abstract IEnumerator Animate();
		protected abstract void OnStop();
		public abstract void DisableTarget();

		[Button(nameof(Reset))]
		protected abstract void Reset();

		public abstract void SetValue(float f);

		protected virtual float SetTime()
		{
			var newT = 0f;
			var time = Time.time * data.Speed;
#if UNITY_EDITOR
			if (!Application.isPlaying) time = Time.realtimeSinceStartup * data.Speed;
#endif

			var tt = (time - startTime) / data.Period;
			newT = Mathf.Clamp01(data.invert ? 1f - tt : tt);
			return newT;
		}

		public void invertWithChild(bool newInvert)
		{
			data.invert = newInvert;
			if (child != null && child.Length > 0)
				for (int i = 0; i < child.Length; i++)
					child[i].data.invert = newInvert;
		}

		public void StopSound()
		{
			if (AS) AS.Stop();
		}

		public void PlaySound()
		{
			var useSound = data.type.HasFlag(AnimationType.Sound);
			if (useSound)
			{
//            #if UNITY_EDITOR
//            Debug.LogError($"play sound {gameObject.name} {(data.sound.GetClip() ? data.sound.GetClip().name : "null")} ",gameObject);
//            #endif

				if (AS) AS.Play();
			}
		}

		protected void InvokeOnTime()
		{
			OnTime?.Invoke();
		}

		protected virtual void InvokeOnFinish()
		{
			if (HasFinishAction) OnFinish();
			OnFinishAction?.Invoke();
		}

		protected virtual void OnDisable()
		{
			Stop();
		}
	}
}