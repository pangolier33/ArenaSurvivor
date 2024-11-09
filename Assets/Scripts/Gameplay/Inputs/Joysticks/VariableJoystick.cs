using UnityEngine;

namespace Bones.Gameplay.Inputs.Joysticks
{
	public class VariableJoystick : Joystick
	{
		[SerializeField] private float moveThreshold = 1;
		[SerializeField] private bool hideJoystick;
		[SerializeField] private JoystickPattern joystickPattern = JoystickPattern.Fixed;
		//alex 04/07/24
		[Header("Set as a part of the 'Move speed'")]
		[SerializeField] float minHeroSpeed = 0.5f;
		[Header("Set as a part of the 'Handle range'")]
		[SerializeField] float _offsetHandlerForMaxSpeed = 0.7f;

		private Vector2 _fixedPosition = Vector2.zero;
		private bool _isPressed;

		public void SetPattern(JoystickPattern joystickPattern)
		{
			this.joystickPattern = joystickPattern;
			if (joystickPattern == JoystickPattern.Fixed)
			{
				background.anchoredPosition = _fixedPosition;
				if (hideJoystick) background.gameObject.SetActive(true);
			}
			else if (hideJoystick) background.gameObject.SetActive(false);
		}

		override protected void OnEnable()
		{
			base.OnEnable();
			_fixedPosition = background.anchoredPosition;
			SetPattern(joystickPattern);
		}

		override protected void OnDisable()
		{
			base.OnDisable();
			_isPressed = false;
		}

		private void Update()
		{
			if (!_isPressed) return;

			if (joystickPattern == JoystickPattern.Dynamic && input.magnitude > moveThreshold)
			{
				background.position =
					Vector3.MoveTowards(background.position, currentPosition, canvas.scaleFactor * 10);
			}
		}

		public override void OnDown(Vector2 position)
		{
			_isPressed = true;
			if (joystickPattern == JoystickPattern.Fixed)
			{
				background.anchoredPosition = startPosition;
			}
			else
			{
				background.localPosition = ScreenPointToAnchoredPosition(position);
			}

			if (hideJoystick) background.gameObject.SetActive(true);

			base.OnDown(position);
			centerPosition = position;
		}

		public override void OnUp(Vector2 position)
		{
			_isPressed = false;
			if (joystickPattern != JoystickPattern.Fixed && hideJoystick) background.gameObject.SetActive(false);

			background.anchoredPosition = startPosition;
			base.OnUp(startPosition);
			input = default;
			JoySpeedModifier = 0;
		}

		public override void OnDrag(Vector2 dragPosition)
		{
			cam = null;

			currentPosition = dragPosition;

			if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
				cam = canvas.worldCamera;

			Vector2 position = centerPosition;
			var radius = Mathf.Max(background.sizeDelta.x, background.sizeDelta.y) / 2;
			input = (dragPosition - position) / radius;
			HandleInput(input.magnitude, input.normalized, radius, cam);
			handle.anchoredPosition = input * radius * handleRange;

			//float distanceHandlerFromCenter = Mathf.Clamp((dragPosition - position).magnitude,0,radius);
			//SpeedModifier = distanceHandlerFromCenter > radius ? 1 : Mathf.Sqrt(distanceHandlerFromCenter) / 10;


			//SpeedModifier = distanceHandlerFromCenter / radius;
			JoySpeedModifier = GetSpeedModifier((dragPosition - position).magnitude,radius);
			//print($"r = {radius}; distFromCenter = {(dragPosition - position).magnitude}; k = {JoySpeedModifier}");
			OnDragged?.Invoke(Direction, JoySpeedModifier);
		}

		private float GetSpeedModifier(float offsetPosition, float JoyRadius)
		{
			if (offsetPosition >= JoyRadius * handleRange * (_offsetHandlerForMaxSpeed)) return 1;
			if (offsetPosition <= JoyRadius * deadZone) return 0;

			var k = (offsetPosition - (JoyRadius * deadZone)) / ((_offsetHandlerForMaxSpeed - deadZone) * JoyRadius);
			return Mathf.Max(deadZone,k);
		}

		protected Vector2 ScreenPointToAnchoredPosition(Vector2 screenPosition)
		{
			if (RectTransformUtility.ScreenPointToLocalPointInRectangle(baseRect, screenPosition, cam, out var anchoredPoint))
			{
				if (cam)
				{
					Vector2 pivotOffset = baseRect.pivot * baseRect.sizeDelta;
					return anchoredPoint - background.anchorMax * baseRect.sizeDelta + pivotOffset;
				}
				return anchoredPoint;
			}

			return Vector2.zero;
		}
	}
}