using Sirenix.OdinInspector;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Plugins.Runtime.UI
{
	[ExecuteInEditMode]
	public class SelectableColorer : MonoBehaviour
	{
		private const string DEFAULT = "@!" + nameof(cloneColors) + " && !" + nameof(selectionOnly);
		private const string SELECTION_ONLY = "@!" + nameof(cloneColors);
		[SerializeField] private Graphic graphic;
		[SerializeField] private bool cloneColors;
		[SerializeField] private bool selectionOnly;

		[SerializeField, ShowIf(SELECTION_ONLY)] private Color NormalColor = Color.magenta;
		[SerializeField, ShowIf(DEFAULT)] private Color PressedColor = Color.magenta;
		[SerializeField, ShowIf(DEFAULT)] private Color HighlightedColor = Color.magenta;
		[SerializeField, ShowIf(SELECTION_ONLY)] private Color SelectedColor = Color.magenta;
		[SerializeField, ShowIf(DEFAULT)] private Color DisabledColor = Color.grey;

		private Image image;
		private Selectable selectable;
		private Sprite previousSprite = null;

		public ColorBlock colors
		{
			get => new()
			{
				colorMultiplier = 1,
				fadeDuration = .1f,
				normalColor = NormalColor,
				highlightedColor = HighlightedColor,
				pressedColor = PressedColor,
				selectedColor = SelectedColor,
				disabledColor = DisabledColor,
			};
			set
			{
				NormalColor = value.normalColor;
				HighlightedColor = value.highlightedColor;
				PressedColor = value.pressedColor;
				SelectedColor = value.selectedColor;
				DisabledColor = value.disabledColor;
			}
		}

		private void OnEnable()
		{
			selectable = GetComponentInParent<Selectable>();

			if (!graphic)
			{
				graphic = GetComponentInChildren<TMP_Text>();

				if (!graphic)
				{
					graphic = GetComponentInChildren<Text>();
				}
			}

			if (!graphic)
			{
				enabled = false;
				return;
			}

			image = selectable.targetGraphic as Image;

			if (NormalColor == Color.magenta) NormalColor = GetColor();
			if (PressedColor == Color.magenta) PressedColor = GetColor();
			if (HighlightedColor == Color.magenta) HighlightedColor = GetColor();
			if (SelectedColor == Color.magenta) SelectedColor = GetColor();
			if (DisabledColor == Color.magenta) DisabledColor = GetColor();
		}

		private void Update()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying && PrefabUtility.IsPartOfPrefabAsset(this))
			{
				return;
			}
#endif
			if (cloneColors)
			{
				SetColor(selectable.targetGraphic.canvasRenderer.GetColor());
				return;
			}

			switch (selectable.transition)
			{
				case Selectable.Transition.SpriteSwap:
					{
						if (image && image.overrideSprite != previousSprite)
						{
							previousSprite = image.overrideSprite;
							UpdateColor();
						}

						break;
					}
				case Selectable.Transition.ColorTint:
					{
						UpdateColorTint();
						break;
					}
			}
		}

		private void UpdateColorTint()
		{
			Color color = Color.white;
			if (selectionOnly)
			{
				color = selectable.hasSelection ? SelectedColor : NormalColor;
			}
			else
			{
				color = selectable.currentSelectionState switch
				{
					Selectable.SelectionState.Normal => NormalColor,
					Selectable.SelectionState.Highlighted => HighlightedColor,
					Selectable.SelectionState.Pressed => PressedColor,
					Selectable.SelectionState.Selected => SelectedColor,
					Selectable.SelectionState.Disabled => DisabledColor,
					_ => NormalColor,
				};
			}

			SetColor(color);
		}

		private void UpdateColor()
		{
			if (selectable.transition == Selectable.Transition.SpriteSwap)
			{
				Sprite sprite = image.overrideSprite;
				if (sprite == selectable.spriteState.pressedSprite)
				{
					SetColor(PressedColor);
				}
				else if (sprite == selectable.spriteState.highlightedSprite)
				{
					SetColor(HighlightedColor);
				}
				else if (sprite == selectable.spriteState.disabledSprite)
				{
					SetColor(DisabledColor);
				}
				else
				{
					SetColor(NormalColor);
				}
			}
		}

		private Color GetColor()
		{
			if (graphic != null) return graphic.color;
			return Color.black;
		}

		private void SetColor(Color color)
		{
			if (graphic != null) graphic.canvasRenderer.SetColor(color);
		}

		#if UNITY_EDITOR
		[MenuItem("CONTEXT/SelectableColorer/Copy Color Block")]
		public static void CopyBlock2(MenuCommand command)
		{
			EditorGUIUtility.systemCopyBuffer = JsonUtility.ToJson((command.context as SelectableColorer).colors);
		}

		[MenuItem("CONTEXT/SelectableColorer/Paste Color Block")]
		public static void PasteBlock2(MenuCommand command)
		{
			Undo.RecordObject(command.context, "paste colors");
			(command.context as SelectableColorer).colors = JsonUtility.FromJson<ColorBlock>(EditorGUIUtility.systemCopyBuffer);
		}
		#endif
	}
}