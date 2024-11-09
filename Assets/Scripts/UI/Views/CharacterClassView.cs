using Bones.Gameplay.Meta.CharacterClases;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static Bones.Gameplay.Spells.Classes.ClassContainer;

namespace Bones.UI
{
	public class CharacterClassView : MonoBehaviour
	{
		[SerializeField] private Image _icon;
		[SerializeField] private SkillClassView[] _skills;

		public void Fill(ICharacterClass characterClass)
		{
			_icon.sprite = characterClass.Icon;
			var tempSkillContainer = _skills.ToList();
			foreach (var skill in characterClass.Skills)
			{
				var skillView = tempSkillContainer.First(x => x.Type == skill.Type);
				skillView.Fill(skill, IsAvailable(skill, characterClass.Skills));
				tempSkillContainer.Remove(skillView);
			}
		}

		private bool IsAvailable(Skill skill, IEnumerable<Skill> skills)
		{
			if (skill.SpellBranch.IsActive)
				return false;
			if (skill.Type == SpellType.Base)
				return true;
			if (skill.Type == SpellType.Common && skills.Any(x => x.Type == SpellType.Base && x.SpellBranch.IsActive))
				return true;
			if (skill.Type == SpellType.Ult && skills.Any(x => x.Type == SpellType.Common && x.SpellBranch.IsActive))
				return true;

			return false;
		}
	}
}