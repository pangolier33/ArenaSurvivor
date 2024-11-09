using Bones.Gameplay.Meta.CharacterClases;
using Bones.UI.Views;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Bones.UI
{
	public class CharacterClassSelector : MonoBehaviour
	{
		[SerializeField] private MetaCharacterClassView _selectedCharacterClass;
		[SerializeField] private MetaCharacterClassView[] _activeClasses;

		[Inject] private ICharacterClassStorage _characterClassStorage;

		public event Action Equipped;

		private void Start()
		{
			foreach (var activeClass in _activeClasses)
				activeClass.Clicked += OnActiveClassClicked;
		}

		private void OnActiveClassClicked(MetaCharacterClassView activeClassView)
		{
			if (activeClassView.CharacterClass != null)
				_characterClassStorage.DeactivateClass(activeClassView.CharacterClass.Name);
			_characterClassStorage.ActivateClass(_selectedCharacterClass.CharacterClass.Name);
			gameObject.SetActive(false);
			Equipped?.Invoke();
		}

		public void SetSelectedCharacterClass(ICharacterClass characterClass)
		{
			_selectedCharacterClass.Initialize(characterClass);
			SetActiveCharacterClass(_characterClassStorage.ActiveClasses);
		}

		private void SetActiveCharacterClass(IEnumerable<ICharacterClass> activeClasses)
		{
			int index = 0;
			foreach (ICharacterClass characterClass in activeClasses)
				_activeClasses[index++].Initialize(characterClass);
		}
	}
}