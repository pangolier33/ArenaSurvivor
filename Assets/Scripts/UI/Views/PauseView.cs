using Bones.Gameplay.Meta.CharacterClases;
using Bones.Gameplay.Spells.Classes;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Bones.UI.Views
{
	public class PauseView : MonoBehaviour
	{
		[SerializeField] private CharacterClassView[] _characterClassesView;
		[SerializeField] private TMP_Text _characterLevel;

		[Inject] private readonly ICharacterClassStorage _characterClassStorage;

		private void OnEnable()
		{
			FillCharacterClasses(_characterClassStorage.ActiveClasses);
			int characterLevel = _characterClassStorage.ActiveClasses.Sum(x => x.Skills.Sum(x => x.SpellBranch.Model.LevelValue())) + 1;
			_characterLevel.SetText(characterLevel.ToString());
		}

		public void FillCharacterClasses(IEnumerable<ICharacterClass> characterClasses)
		{
			int index = 0;
			foreach (var characterClass in characterClasses)
				_characterClassesView[index++].Fill(characterClass);
		}

		public void SwitchToMainMenu()
		{
			SceneManager.LoadScene("Menu");
		}

		public void Reastart()
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}
	}
}