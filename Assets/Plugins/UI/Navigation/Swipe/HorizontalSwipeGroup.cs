using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Navigation.Swipe
{
	public class HorizontalSwipeGroup : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
	{
		private const string INVALID_VIEWPORT = "Viewport should parent or upper then current Transform";
		[SerializeField] private AnimationCurve smoothCurve;
		[SerializeField] private float smoothTime;
		[SerializeField] private float minSwipeDistance = 50;
		[SerializeField] private int initialSelectedIndex;
		[SerializeField] private bool _resizeWidth = true;
		[SerializeField] public RectTransform ScrollRect;

		[SerializeField, Range(.1f, 1)] private float _distance = .5f;
		[SerializeField, Required, InfoBox(INVALID_VIEWPORT, nameof(ValidateViewport))] private RectTransform viewport;
		[SerializeField, Required] private Transform canvas;
		[ShowInInspector] private bool IsSwiping => _switchingCor != null;
		private ISwipeGroupElement[] _targets;
		private int _currentSelected = -1;
		private Coroutine _switchingCor;
		private Vector2 _startPos;
		private bool _canSwitch;

		private bool ValidateViewport() => ScrollRect == viewport;

		//private void Start() => Init();
		public void Init(MonoBehaviour runner) => runner.StartCoroutine(InitCoroutine());

		private IEnumerator InitCoroutine()
		{
			ScrollRect ??= GetComponent<RectTransform>();
			_targets = ScrollRect.GetComponentsInChildren<ISwipeGroupElement>(true);
			for (int i = 0; i < _targets.Length; i++)
			{
				if (_targets[i].rectTransform)
				{
					_targets[i].rectTransform.position = ScrollRect.position + Vector3.right * viewport.rect.width * i * canvas.localScale.x * _distance;
					if (_resizeWidth)
					{
					}
				}
			}

			// пропускаем 4 кадра, чтобы SizeFitterы сработали
			for (int i = 0; i < 4; i++) yield return null;

			Select(initialSelectedIndex);
			for (int i = 0; i < _targets.Length; i++)
			{
				if (i == initialSelectedIndex)
				{
					_targets[i].OnSelected();
				}
				else
				{
					_targets[i].OnDeselected();
					_targets[i].OnDeselectEnd();
				}
			}

			float deltaPos = _targets[_currentSelected].rectTransform.position.x - viewport.position.x;

			ScrollRect.position -= Vector3.right * deltaPos;

			for (int i = 0; i < _targets.Length; i++)
			{
				_targets[i].PostInit();
			}
		}

		[Button]
		public void Select(int index, MonoBehaviour runner = null)
		{
			if (runner == null)
			{
				runner = this;
			}

			if (index < 0 || index >= _targets.Length) return;

			if (index != _currentSelected)
			{
				if (_currentSelected >= 0)
				{
					_targets[_currentSelected].OnDeselected();
				}

				for (int i = 0; i < _targets.Length; i++)
				{
					_targets[i].OnSelectSwitch();
				}

				_targets[index].OnSelected();
				_currentSelected = index;

				if (_switchingCor != null)
				{
					runner.StopAllCoroutines();
				}
				if (runner.gameObject.activeSelf && gameObject.activeSelf)
					_switchingCor = runner.StartCoroutine(SelectCor());
			}
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
			_startPos = eventData.position;
			_canSwitch = true;
		}

		public void OnEndDrag(PointerEventData eventData)
		{
		}

		public void OnDrag(PointerEventData eventData)
		{
			if (!_canSwitch) return;
			if (eventData.position.x - _startPos.x > minSwipeDistance)
			{
				Select(_currentSelected - 1);
			}

			if (_startPos.x - eventData.position.x > minSwipeDistance)
			{
				Select(_currentSelected + 1);
			}
		}

		private IEnumerator SelectCor()
		{
			_canSwitch = false;
			float startPos = ScrollRect.position.x;
			float endPos = ScrollRect.position.x - _targets[_currentSelected].rectTransform.position.x + viewport.position.x;
			;
			float t = 0;

			while (t < smoothTime)
			{
				t += Time.deltaTime;
				float normalized = Mathf.Clamp01(t / smoothTime);

				ScrollRect.position = new Vector2(Mathf.Lerp(startPos, endPos, smoothCurve.Evaluate(normalized)), viewport.position.y);

				yield return null;
			}

			for (int i = 0; i < _targets.Length; i++)
			{
				if (i != _currentSelected)
				{
					_targets[i].OnDeselectEnd();
				}
			}

			_switchingCor = null;
		}

		private void OnDestroy()
		{
			if (IsSwiping)
			{
				StopCoroutine(_switchingCor);
			}
		}
	}
}