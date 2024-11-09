using Assets.Scripts.UI.Presenters;
using Assets.Scripts.UI.Views;
using Bones.Gameplay.Meta;
using Bones.Gameplay.Meta.CharacterClases;
using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Bones.UI
{
	public class CharacterClassInfoView : MonoBehaviour
	{
		[SerializeField] private Image _icon;
		[SerializeField] private TMP_Text _level;
		[SerializeField] private TMP_Text _name;
		[SerializeField] private TMP_Text _rare;
		[SerializeField] private TMP_Text _description;
		[SerializeField] private TMP_Text _element;
		[SerializeField] private MetaSkillView[] _skills;
		[SerializeField] private SelectedMetaSkillView _selectedSkill;
		[SerializeField] private Button _equipButton;
		[SerializeField] private TMP_Text _currentMultiplier;
		[SerializeField] private TMP_Text _nextMultiplier;
		[SerializeField] private MetaCharacterClassUpgrader _metaCharacterClassUpgrader;
		[SerializeField] private CharacterClassSelector _characterClassSelector;

		[Inject] private ICharacterClassStorage _characterClassStorage;

		private ICharacterClass _characterClass;

		private void Start()
		{
			foreach (var skill in _skills)
				skill.Clicked += OnSkillClicked;
			_metaCharacterClassUpgrader.Upgraded += OnUpgraded;
			_equipButton.onClick.AddListener(EquipCharacterClass);
			_characterClassSelector.Equipped += OnEquipeed;
		}

		private void OnEquipeed()
		{
			if (_characterClass != null)
				Initialize(_characterClass);
		}

		private void EquipCharacterClass()
		{
			_characterClassSelector.SetSelectedCharacterClass(_characterClass);
			_characterClassSelector.gameObject.SetActive(true);
		}

		private void OnUpgraded()
		{
			Initialize(_characterClass);
		}

		private void OnSkillClicked(MetaSkillView skillView)
		{
			_selectedSkill.Initialize(skillView.Skill);
			foreach (var skill in _skills)
				skill.DeactivateBackground();
			skillView.ActivateBackground();
		}

		public void Initialize(ICharacterClass characterClass)
		{
			_characterClass = characterClass;
			_icon.sprite = characterClass.Icon;
			_level.text = characterClass.Level.ToString();
			_name.text = characterClass.Name.ToString();
			_rare.text = characterClass.Rare.ToString();
			_rare.color = RareColorDeterminator.GetColor(characterClass.Rare);
			_currentMultiplier.text = characterClass.DamageMultiplier + "%";
			_nextMultiplier.text = characterClass.NextLevelDamageMultiplier + "%";

			_metaCharacterClassUpgrader.Initialize(characterClass);
			FillSkills(characterClass);

			if (_characterClassStorage.IsActive(characterClass.Name))
				_equipButton.interactable = false;
			else
				_equipButton.interactable = true;
		}


		private void FillSkills(ICharacterClass characterClass)
		{
			int index = 0;
			foreach (var skill in characterClass.Skills) 
			{
				_skills[index].Initialize(skill);
				_skills[index].DeactivateBackground();
				index++;
			}
			_selectedSkill.Initialize(characterClass.Skills.First());
			_skills.First().ActivateBackground();
		}
	}
}