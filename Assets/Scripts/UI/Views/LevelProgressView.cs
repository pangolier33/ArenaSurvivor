using Bones.Gameplay.Meta.CharacterClases;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Bones.UI
{
	public class LevelProgressView : MonoBehaviour
	{
		[SerializeField] private TMP_Text _current;
		[SerializeField] private TMP_Text _total;
		[SerializeField] private Image _value;
		[SerializeField] private Color _nonFilled;
		[SerializeField] private Color _filled;

		public void Initialize(ICharacterClass characterClass)
		{
			_current.text = characterClass.Experience.Value.ToString();
			_total.text = characterClass.ExperienceToNextLevel.ToString();
			_value.fillAmount = characterClass.LevelUpProgress;

			if (characterClass.ReadyToLevelUp)
				_value.color = _filled;
			else
				_value.color = _nonFilled;
		}
	}
}
