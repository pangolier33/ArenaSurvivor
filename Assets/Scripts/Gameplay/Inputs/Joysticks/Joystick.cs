using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Bones.Gameplay.Inputs.Joysticks
{
    public class Joystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler, IDirectionalInput
    {
        public Vector2 Direction => input;

		public float JoySpeedModifier { get; protected set; } = 1f;

		[SerializeField] protected float handleRange = 1;
        [SerializeField] protected float deadZone;
        [SerializeField] protected bool snapX;
        [SerializeField] protected bool snapY;

        [SerializeField] protected RectTransform background;
        [SerializeField] protected RectTransform handle;
        [SerializeField] protected InputActionReference wasd;

        protected RectTransform baseRect;

        protected Canvas canvas;
        protected UnityEngine.Camera cam;

        protected Vector2 input = Vector2.zero;
        protected Vector2 startPosition = Vector2.zero;
        protected Vector2 centerPosition = Vector2.zero;
        protected Vector2 currentPosition = Vector2.zero;

        public Action<Vector2,float> OnDragged;

        protected virtual void OnEnable()
        {
            baseRect = GetComponent<RectTransform>();
            canvas = GetComponentInParent<Canvas>();
            if (canvas == null)
                Debug.LogError("The Joystick is not placed inside a canvas");

            Vector2 center = new Vector2(0.5f, 0.5f);
            background.pivot = center;
            handle.anchorMin = center;
            handle.anchorMax = center;
            handle.pivot = center;
            handle.anchoredPosition = Vector2.zero;

            OnDisable();

            if (wasd)
            {
                wasd.action.Enable();
                wasd.action.started += OnDownWasd;
                wasd.action.performed += OnDragWasd;
                wasd.action.canceled += OnUpWasd;
            }

            if (startPosition == default)
            {
                startPosition = background.anchoredPosition;
            }
        }

        protected virtual void OnDisable()
        {
            if (wasd)
            {
                wasd.action.Disable();
                wasd.action.started -= OnDownWasd;
                wasd.action.performed -= OnDragWasd;
                wasd.action.canceled -= OnUpWasd;
            }
        }

        // UI
        public void OnPointerDown(PointerEventData eventData) => OnDown(eventData.position);

        public void OnDrag(PointerEventData eventData) => OnDrag(eventData.position);

        public virtual void OnPointerUp(PointerEventData eventData) => OnUp(eventData.position);

        // WASD

        void OnDownWasd(InputAction.CallbackContext context)
        {
            background.anchoredPosition = startPosition;
            OnDown(startPosition);
        }

        void OnUpWasd(InputAction.CallbackContext context) => OnUp(startPosition);

        void OnDragWasd(InputAction.CallbackContext context)
        {
            Vector2 position = (Vector2) background.position + context.ReadValue<Vector2>() * background.sizeDelta;
            OnDrag(position);
            OnDrag(position);
        }

        // Shared

        public virtual void OnDown(Vector2 position)
        {
            centerPosition = position;
            OnDrag(position);
        }

        public virtual void OnUp(Vector2 position)
        {
            OnDrag(background.localPosition);
            input = Vector2.zero;
            handle.anchoredPosition = Vector2.zero;
            centerPosition = startPosition;
        }

        public virtual void OnDrag(Vector2 dragPosition)
        {
            cam = null;

            currentPosition = dragPosition;

            if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
                cam = canvas.worldCamera;

            Vector2 position = centerPosition;
            var radius = Mathf.Max(background.sizeDelta.x, background.sizeDelta.y) / 2;
            input = (dragPosition - position) / (radius * canvas.scaleFactor);
            HandleInput(input.magnitude, input.normalized, radius, cam);
            handle.anchoredPosition = input * radius * handleRange;
            OnDragged?.Invoke(Direction,JoySpeedModifier);
        }

        protected virtual void HandleInput(float magnitude, Vector2 normalized, float radius, UnityEngine.Camera cam)
        {
            if (magnitude > deadZone)
            {
                if (magnitude > 1)
                    input = normalized;
            }
            else
                input = Vector2.zero;
        }
    }
}