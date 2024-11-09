using Bones.Gameplay.Meta.CharacterClases;
using Bones.UI;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Assets.Scripts.UI.Presenters
{
	public class MetaCharacterClassUpgrader : MonoBehaviour
	{
		[SerializeField] private RareProgressView _rareProgressView;
		[SerializeField] private LevelProgressView[] _levelProgressViews;
		[SerializeField] private Button _evolveButton;
		[SerializeField] private Button _improveButton;
		[SerializeField] private Transform _evolve;
		[SerializeField] private Transform _improve;
		[SerializeField] private TMP_Text _improveValue;
		[SerializeField] private TMP_Text _evolveValue;

		[Inject] private CharacterClassUpgrader _characterClassUpgrader;

		public event Action Upgraded;

		private ICharacterClass _characterClass;

		private void Start()
		{
			_evolveButton.onClick.AddListener(Evolve);
			_improveButton.onClick.AddListener(Improve);
		}

		private void Improve()
		{
			if (!_characterClassUpgrader.EnoughSilverToLevelUp(_characterClass.Name))
				return;

			_characterClassUpgrader.UpLevel(_characterClass.Name);

			UpdateProgress();
			Upgraded?.Invoke();
		}

		private void Evolve()
		{
			if (!_characterClassUpgrader.EnoughSilverToRareUpgrade(_characterClass.Name))
				return;

			_characterClassUpgrader.UpgradeRare(_characterClass.Name);

			UpdateProgress();
			Upgraded?.Invoke();
		}

		public void Initialize(ICharacterClass characterClass)
		{
			_characterClass = characterClass;
			UpdateProgress();
		}

		private void UpdateProgress()
		{
			_rareProgressView.Initialize(_characterClass);
			foreach (var levelProgressView in _levelProgressViews)
				levelProgressView.Initialize(_characterClass);
			if (NextUpgradeIsEvolve())
			{
				_evolve.gameObject.SetActive(true);
				_improve.gameObject.SetActive(false);
				_evolveButton.interactable = _characterClass.ReadyToUpgradeRare;
				_evolveValue.text = _characterClass.SoftToUpgradeRare.ToString();
			}
			else
			{
				_evolve.gameObject.SetActive(false);
				_improve.gameObject.SetActive(true);
				_improveButton.interactable = _characterClass.ReadyToLevelUp;
				_improveValue.text = _characterClass.SoftToLevelUp.ToString();
			}
		}

		private bool NextUpgradeIsEvolve()
		{
			return _characterClass.Level % 10 == 0
				&& (int)_characterClass.Rare <= _characterClass.Level / 10;
		}
	}
}