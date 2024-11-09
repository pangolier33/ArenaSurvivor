using Bones.Gameplay.Meta.CharacterClases;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Views
{
	public class MetaSkillView : MonoBehaviour
	{
		[SerializeField] private Image _icon;
		[SerializeField] private Image _background;
		[SerializeField] private Button _skillButton;

		public event Action<MetaSkillView> Clicked;

		public Skill Skill { get; private set; }

		private void Awake()
		{
			_skillButton.onClick.AddListener(() => Clicked?.Invoke(this));
		}

		public void Initialize(Skill skill)
		{
			Skill = skill;
			_icon.sprite = skill.Icon;
		}

		public void ActivateBackground()
		{
			_background.color = Color.yellow;
		}

		public void DeactivateBackground()
		{
			_background.color = new Color(0, 0, 0, 0);
		}
	}
}