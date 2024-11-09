using Bones.Gameplay.Meta.CharacterClases;
using UnityEngine;
using UnityEngine.UI;
using static Bones.Gameplay.Spells.Classes.ClassContainer;

namespace Bones.UI
{
	public class SkillClassView : MonoBehaviour
	{
		[SerializeField] private Image _background;
		[SerializeField] private Image _icon;
		[SerializeField] private Color _availableColor;
		[SerializeField] private SpellType _type;
		[SerializeField] private Color _unavailableColor;
		
		private Color? _initialBackroundColor;

		public SpellType Type => _type;

		public void Fill(Skill skill, bool available)
		{
			if (_initialBackroundColor == null)
				_initialBackroundColor = _background.color;

			if (skill.SpellBranch.IsActive)
			{
				_icon.sprite = skill.Icon;
				_background.color = _initialBackroundColor.Value;
				_icon.color = Color.white;
			}
			else if (available) 
			{
				_icon.sprite = ConvertToBluescale(skill.Icon);
				_background.color = _initialBackroundColor.Value;
				_icon.color = _availableColor;
			}
			else
			{
				_icon.sprite = ConvertToGrayscale(skill.Icon);
				_background.color = _unavailableColor;
				_icon.color = Color.white;
			}
		}

		public static Sprite ConvertToGrayscale(Sprite sprite)
		{
			Texture2D graph = new (sprite.texture.width, sprite.texture.height);
			graph.SetPixels(sprite.texture.GetPixels());
			graph.Apply();
			Color32[] pixels = graph.GetPixels32();
			for (int x = 0; x < graph.width; x++)
			{
				for (int y = 0; y < graph.height; y++)
				{
					Color32 pixel = pixels[x + y * graph.width];
					if (pixel.a == 0)
					{
						graph.SetPixel(x, y, pixel);
					}
					else
					{
						int p = ((256 * 256 + pixel.r) * 256 + pixel.b) * 256 + pixel.g;
						int b = p % 256;
						p = Mathf.FloorToInt(p / 256);
						int g = p % 256;
						p = Mathf.FloorToInt(p / 256);
						int r = p % 256;
						float l = (0.2126f * r / 255f) + 0.7152f * (g / 255f) + 0.0722f * (b / 255f);
						Color c = new Color(l, l, l, 1);
						graph.SetPixel(x, y, c);
					}
				}
			}
			graph.Apply(true);
			Sprite graySprite = Sprite.Create(graph, new Rect(0, 0, graph.width, graph.height), new Vector2(graph.width / 2, graph.height / 2));
			return graySprite;
		}

		public static Sprite ConvertToBluescale(Sprite sprite)
		{
			Texture2D graph = new(sprite.texture.width, sprite.texture.height);
			graph.SetPixels(sprite.texture.GetPixels());
			graph.Apply();
			Color32[] pixels = graph.GetPixels32();
			for (int x = 0; x < graph.width; x++)
			{
				for (int y = 0; y < graph.height; y++)
				{
					Color32 pixel = pixels[x + y * graph.width];
					if (pixel.a == 0)
					{
						graph.SetPixel(x, y, pixel);
					}
					else
					{
						int p = ((256 * 256 + pixel.r) * 256 + pixel.b) * 256 + pixel.g;
						int b = p % 256;
						p = Mathf.FloorToInt(p / 256);
						int g = p % 256;
						p = Mathf.FloorToInt(p / 256);
						int r = p % 256;
						float l = (0.6f * r / 255f) + 0.7f * (g / 255f) + 0.922f * (b / 255f);
						Color c = new Color(l, l, l, 1);
						graph.SetPixel(x, y, c);
					}
				}
			}
			graph.Apply(true);
			Sprite graySprite = Sprite.Create(graph, new Rect(0, 0, graph.width, graph.height), new Vector2(graph.width / 2, graph.height / 2));
			return graySprite;
		}
	}
}
