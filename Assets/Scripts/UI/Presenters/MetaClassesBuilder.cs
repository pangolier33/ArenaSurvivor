using Bones.Gameplay.Meta.CharacterClases;
using Bones.UI.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Bones.UI.Presenters
{
	public class MetaClassesBuilder : MonoBehaviour
	{
		[SerializeField] private MetaCharacterClassView _prefab;
		[SerializeField] private Transform _allClassesParrent;
		[SerializeField] private MetaCharacterClassView[] _activeClasssesView;
		[SerializeField] private CharacterClassSelector _characterClassSelector;
		[SerializeField] private CharacterClassInfoView _characterClassInfoView;

		[Inject] private ICharacterClassStorage _characterClassStorage;

		private List<MetaCharacterClassView> _views = new ();
		private MetaCharacterClassView _selected;

		private void Start()
		{
			_characterClassSelector.gameObject.SetActive(false);
			_characterClassInfoView.gameObject.SetActive(false);

			foreach (var characterClass in _characterClassStorage.AllClasses)
			{
				var metaCharacterClassView = Instantiate(_prefab, _allClassesParrent);
				metaCharacterClassView.Initialize(characterClass);
				metaCharacterClassView.Clicked += OnClicked;
				metaCharacterClassView.Selected += OnSelected;
				metaCharacterClassView.CalledInfo += OnCalledInfo;
				_views.Add(metaCharacterClassView);
			}
			foreach (var activeClass in _activeClasssesView)
				activeClass.Clicked += OnActiveClassClicked;	
			UpdateActiveClasses();
			UpdateAllClasses();
			_characterClassStorage.ActiveClassChanged += UpdateActiveClasses;
		}

		private void OnActiveClassClicked(MetaCharacterClassView activeClassView)
		{
			if (activeClassView.CharacterClass != null)
			{
				_characterClassInfoView.Initialize(activeClassView.CharacterClass);
				_characterClassInfoView.gameObject.SetActive(true);
			}
		}

		private void OnCalledInfo(ICharacterClass characterClass)
		{
			_characterClassInfoView.Initialize(characterClass);
			_characterClassInfoView.gameObject.SetActive(true);
		}

		private void OnEnable()
		{
			UpdateActiveClasses();
			UpdateAllClasses();
		}

		private void OnSelected(ICharacterClass characterClass)
		{
			_characterClassSelector.SetSelectedCharacterClass(characterClass);
			_characterClassSelector.gameObject.SetActive(true);
		}

		private void UpdateActiveClasses(IEnumerable<ICharacterClass> activeClasses)
		{
			UpdateActiveClasses();
			UpdateAllClasses();
		}

		private void UpdateActiveClasses()
		{
			int index = 0;
			foreach (var activeClass in _characterClassStorage.ActiveClasses)
				_activeClasssesView[index++].Initialize(activeClass);
		}

		private void OnClicked(MetaCharacterClassView clickedView)
		{
			foreach (var view in _views)
			{
				if (view == clickedView)
					if (_selected == clickedView)
					{
						_selected = null;
						view.Deselect();
					}
					else
					{
						view.Select();
						_selected = view;
					}
				else
					view.Deselect();
			}
		}

		private void UpdateAllClasses()
		{
			foreach (var view in _views)
			{
				if (_characterClassStorage.IsActive(view.CharacterClass.Name))
					view.DisableChoose();
				else
					view.EnableChoose();
				var characterClass = _characterClassStorage.AllClasses.First(x => x.Name == view.CharacterClass.Name);
				view.Initialize(characterClass);
			}
		}
	}
}