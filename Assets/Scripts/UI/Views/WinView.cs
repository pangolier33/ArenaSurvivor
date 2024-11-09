using Bones.Gameplay.Entities;
using Bones.Gameplay.Meta.CharacterClases;
using Bones.Gameplay.Meta.Currencies;
using Bones.Gameplay.Spells.Classes;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Bones.UI
{
	public class WinView : MonoBehaviour
	{
		[SerializeField] private TMP_Text _level;
		[SerializeField] private TMP_Text _killedEnemy;
		[SerializeField] private TMP_Text _silver;

		[Inject] private readonly ICharacterClassStorage _characterClassStorage;
		[Inject] private readonly ICurrencyStorage _currenciesStorage;
		[Inject] private readonly KillCounter _killCounter;

		private void OnEnable()
		{
			int characterLevel = _characterClassStorage.ActiveClasses.Sum(x => x.Skills.Sum(x => x.SpellBranch.Model.LevelValue())) + 1;
			_level.SetText(characterLevel.ToString());
			_currenciesStorage.LevelCurrency.Deposit(_currenciesStorage.LevelCurrency.Value);
			_silver.text = $"+{_currenciesStorage.LevelCurrency.Value}";
			_currenciesStorage.TransferLevelCurrencyToBalance();
			_killedEnemy.text = _killCounter.Count.ToString();
		}

		public void Continue()
		{
			SceneManager.LoadScene("Menu");
		}
	}
}
