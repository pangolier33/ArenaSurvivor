using Bones.Gameplay.Meta.CharacterClases;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Bones.UI
{
	public class SelectedMetaSkillView : MonoBehaviour
	{
		[SerializeField] private Image _icon;
		[SerializeField] private TMP_Text _name;
		[SerializeField] private TMP_Text _description;

		public void Initialize(Skill skill)
		{
			_icon.sprite = skill.Icon;
			_name.text = skill.Name;
			_description.text = skill.Description;
		}
	}
}