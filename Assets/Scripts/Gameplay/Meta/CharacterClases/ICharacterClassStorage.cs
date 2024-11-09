using Bones.Gameplay.Spells.Classes;
using System;
using System.Collections.Generic;

namespace Bones.Gameplay.Meta.CharacterClases
{
	public interface ICharacterClassStorage
	{
		IEnumerable<ICharacterClass> ActiveClasses { get; }
		IEnumerable<ICharacterClass> AllClasses { get; }
		IEnumerable<ICharacterClass> NotOpenedClasses { get; }
		IEnumerable<ICharacterClass> OpenedClasses { get; }

		event Action<IEnumerable<ICharacterClass>> ActiveClassChanged;
		event Action<IEnumerable<ICharacterClass>> OpenedClassChanged;

		void ApplySave(ICharacterClassStorageSave save);
		void ActivateClass(ClassName name);
		void DeactivateClass(ClassName name);
		void OpenClass(ClassName name);
		bool IsActive(ClassName name);
	}
}