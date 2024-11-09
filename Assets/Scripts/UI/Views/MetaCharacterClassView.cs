using Bones.Gameplay.Meta;
using Bones.Gameplay.Meta.CharacterClases;
using Bones.Gameplay.Meta.Equipment;
using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.ProceduralImage;

namespace Bones.UI.Views
{
	public class MetaCharacterClassView : MonoBehaviour
	{
		[SerializeField] private Image _icon;
		[SerializeField] private Image _levelBackground;
		[SerializeField] private TMP_Text _level;
		[SerializeField] private TMP_Text _upgrade;
		[SerializeField] private ProceduralImage _upgradeProgress;
		[SerializeField] private Button _chooseButton;
		[SerializeField] private Button _infoButton;
		[SerializeField] private Button _infoButtonWithPrice;
		[SerializeField] private TMP_Text _infoButtonValue;
		[SerializeField] private Transform _background;
		[SerializeField] private Image _rareBackground;
		[SerializeField] private Transform _rare;
		[SerializeField] private Button _click;
		[SerializeField] private Canvas _canvas;

		private bool _selected;

		public event Action<MetaCharacterClassView> Clicked;
		public event Action<ICharacterClass> Selected;
		public event Action<ICharacterClass> CalledInfo;

		public ICharacterClass CharacterClass { get; private set; }

		private void Start()
		{
			_click.onClick.AddListener(() => Clicked?.Invoke(this));
			_chooseButton.onClick.AddListener(() => Selected?.Invoke(CharacterClass));
			_infoButton.onClick.AddListener(() => CalledInfo?.Invoke(CharacterClass));
			_infoButtonWithPrice.onClick.AddListener(() => CalledInfo?.Invoke(CharacterClass));
		}

		public void Initialize(ICharacterClass characterClass)
		{
			CharacterClass = characterClass;
			_icon.sprite = characterClass.Icon;
			_level.text = $"Lvl {characterClass.Level}";
			_upgrade.text = $"{characterClass.Copies}/{characterClass.CopiesToUpgradeRare}";
			_upgradeProgress.fillAmount = ((float)characterClass.Copies) / characterClass.CopiesToUpgradeRare;
			var backgroundColor = RareColorDeterminator.GetColor(characterClass.Rare);
			_levelBackground.color = backgroundColor;
			_rareBackground.color = new Color(backgroundColor.r, backgroundColor.g, backgroundColor.b, 0.5f);
			if (_selected)
				Select();
		}

		public void Select()
		{
			_selected = true;
			_background.gameObject.SetActive(true);
			_chooseButton.gameObject.SetActive(true);
			if (CharacterClass.ReadyToLevelUp || CharacterClass.ReadyToUpgradeRare)
			{
				_infoButtonWithPrice.gameObject.SetActive(true);
				_infoButton.gameObject.SetActive(false);
				if (CharacterClass.ReadyToLevelUp)
					_infoButtonValue.text = CharacterClass.SoftToLevelUp.ToString();
				else
					_infoButtonValue.text = CharacterClass.SoftToUpgradeRare.ToString();
			}
			else
			{
				_infoButton.gameObject.SetActive(true);
				_infoButtonWithPrice.gameObject.SetActive(false);
			}
			_rareBackground.gameObject.SetActive(false);
			_rare.gameObject.SetActive(false);
			_canvas.sortingOrder = 2;
		}

		public void Deselect()
		{
			_selected = false;
			_background.gameObject.SetActive(false);
			_chooseButton.gameObject.SetActive(false);
			_infoButton.gameObject.SetActive(false);
			_infoButtonWithPrice.gameObject.SetActive(false);
			_rareBackground.gameObject.SetActive(true);
			_rare.gameObject.SetActive(true);
			_canvas.sortingOrder = 1;
		}

		public void EnableChoose()
		{
			_chooseButton.interactable = true;
		}

		public void DisableChoose()
		{
			_chooseButton.interactable = false;
		}
	}
}