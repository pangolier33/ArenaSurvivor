using Railcar.Time;
using Railcar.UI;
using System;
using System.Collections.Generic;
using Bones.Gameplay.LevelLoader;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;
using Bones.Gameplay.Meta.Currencies;
using TMPro;
using Bones.Gameplay.Meta.CharacterClases;
using Bones.Gameplay.Entities;
using System.Linq;
using Bones.Gameplay.Spells.Classes;

namespace Bones.UI.Menus
{
	public class GameOverMenu : BaseMenu
	{
		[SerializeReference] private BindingBuilder _textBinding;
		[SerializeReference] private BindingBuilder _timeBinding;
		[SerializeReference] private BindingBuilder _restartBinding;
		[SerializeReference] private BindingBuilder _exitBinding;
		[SerializeField] private TMP_Text _silver;
		[SerializeField] private TMP_Text _kills;
		[SerializeField] private TMP_Text _level;

		[SerializeField] private string _endField;
		private IObservableClock _clock;

		[Inject]
		private void Inject([Inject(Id = TimeID.Fixed)] IObservableClock clock,
			ICurrencyStorage currencyStorage,
			ICharacterClassStorage characterClassStorage,
			KillCounter killCounter)
		{
			_clock = clock;
			_silver.text = currencyStorage.LevelCurrency.Value.ToString();
			currencyStorage.TransferLevelCurrencyToBalance();
			int characterLevel = characterClassStorage.ActiveClasses.Sum(x => x.Skills.Sum(x => x.SpellBranch.Model.LevelValue())) + 1;
			_level.SetText(characterLevel.ToString());
			_kills.text = killCounter.Count.ToString();
		}

		protected override IEnumerable<IDisposable> SetupBindings()
		{
			yield return _textBinding.Bind(_endField);
			yield return _timeBinding.Bind((IObservable<float>)_clock.Current);
			yield return _restartBinding.Bind((Action)OnClickedRestart);
			yield return _exitBinding.Bind((Action)OnClickedExitToMenu);
		}

		private static void OnClickedRestart()
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}

		private static void OnClickedExitToMenu()
		{
			SceneManager.LoadScene((int)eScene.Menu);
		}
	}
}