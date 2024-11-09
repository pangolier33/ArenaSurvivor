using Bones.Gameplay.Spells.Classes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bones.Gameplay.Meta.CharacterClases
{
	public class CharacterClassStorage : ICharacterClassStorage
	{
		private const int MAX_ACTIVE_CLASSES = 4;

		private List<ICharacterClass> _activeClasses = new();
		private List<ICharacterClass> _openedClasses = new();
		private List<ICharacterClass> _allClasses = new();

		public event Action<IEnumerable<ICharacterClass>> ActiveClassChanged;
		public event Action<IEnumerable<ICharacterClass>> OpenedClassChanged;

		public IEnumerable<ICharacterClass> ActiveClasses => _activeClasses;
		public IEnumerable<ICharacterClass> OpenedClasses => _openedClasses;
		public IEnumerable<ICharacterClass> NotOpenedClasses => AllClasses.Except(OpenedClasses);
		public IEnumerable<ICharacterClass> AllClasses => _allClasses;

		public CharacterClassStorage(IEnumerable<IClassContainer> baseClasses)
		{
			CreateClasses(baseClasses);
			
		}

		public void ApplySave(ICharacterClassStorageSave save)
		{
			FilledClasses(save.ClassesSave, _allClasses);
			foreach (var className in save.ActiveClassesSave) 
			{
				var activeClass = AllClasses.First(x => x.Name == className);
				_activeClasses.Add(activeClass);
			}
			ActiveClassChanged?.Invoke(ActiveClasses);
		}

		private void CreateClasses(IEnumerable<IClassContainer> baseClasses)
		{
			foreach (var classConteiner in baseClasses)
			{
				CharacterClass characterClass = new(classConteiner);
				_allClasses.Add(characterClass);
			}
		}

		private void FilledClasses(IEnumerable<ICharacterClassSave> classesSave, List<ICharacterClass> classes)
		{
			foreach (var classSave in classesSave)
			{
				var originalCharacterClass = AllClasses.FirstOrDefault(x => x.Name == classSave.Name);
				if (originalCharacterClass == null)
					throw new InvalidOperationException($"Can't find class {classSave.Name} in storage.");

				var copiedClass = new CharacterClass(originalCharacterClass, classSave);
				classes.Add(copiedClass);
				_allClasses.Remove(originalCharacterClass);
			}
		}

		public void ActivateClass(ClassName name)
		{
			//ToDo: пока сделал все классы
			if (!AllClasses.Any(x => x.Name ==name))
				throw new InvalidOperationException($"Class {name} isn't open.");
			if (_activeClasses.Any(x => x.Name == name))
				throw new InvalidOperationException($"Class {name} already activated.");
			if (_activeClasses.Count == MAX_ACTIVE_CLASSES)
				throw new InvalidOperationException($"Max number classes activated ({MAX_ACTIVE_CLASSES}");

			var activatingClass = AllClasses.First(x => x.Name == name);
			_activeClasses.Add(activatingClass);
			ActiveClassChanged?.Invoke(ActiveClasses);
		}

		public void DeactivateClass(ClassName name)
		{
			var deactivatingClass = ActiveClasses.FirstOrDefault(x => x.Name == name);
			if (deactivatingClass == null)
				throw new InvalidOperationException($"Can't deactivate class {name}. Class isn't active.");
			
			_activeClasses.Remove(deactivatingClass);
			ActiveClassChanged?.Invoke(ActiveClasses);
		}

		public void OpenClass(ClassName name)
		{
			if (!AllClasses.Any(x => x.Name == name))
				throw new InvalidOperationException($"Unknown class {name}. Please, use property " +
					$"{nameof(NotOpenedClasses)} to get a list of classes available to open.");
			var openingClass = OpenedClasses.FirstOrDefault(x => x.Name == name);
			if(openingClass == null)
			{
				openingClass = AllClasses.First(x => x.Name == name);
				var copiedClass = new CharacterClass(openingClass);
				_openedClasses.Add(copiedClass);
				OpenedClassChanged?.Invoke(OpenedClasses);
			}
			else
			{
				(openingClass as CharacterClass).AddCopy();
			}
		}

		public bool IsActive(ClassName name)
		{
			return ActiveClasses.Any(x => x.Name == name);
		}

		public void Upgrade()
		{
			ActiveClassChanged?.Invoke(ActiveClasses);
		}
	}
}
