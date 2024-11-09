using Bones.Gameplay.Meta.CharacterClases;
using Bones.Gameplay.Spells.Classes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bones.Gameplay.Saving.Local
{
	[Serializable]
	public class LocalCharacterClassStorageSave : ICharacterClassStorageSave
	{
		public LocalCharacterClassSave[] _classesSave;
		public ClassName[] _activeClassesSave;

		public IEnumerable<ICharacterClassSave> ClassesSave { get => _classesSave; set => _classesSave = value.Cast<LocalCharacterClassSave>().ToArray(); }

		public IEnumerable<ClassName> ActiveClassesSave { get => _activeClassesSave; set => _activeClassesSave = value.ToArray(); }
	}

}