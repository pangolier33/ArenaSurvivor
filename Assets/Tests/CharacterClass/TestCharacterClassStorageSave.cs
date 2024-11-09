using Bones.Gameplay.Meta.CharacterClases;
using Bones.Gameplay.Spells.Classes;
using System.Collections.Generic;

namespace Assets.Tests
{
	public class TestCharacterClassStorageSave : ICharacterClassStorageSave
	{
		public IEnumerable<ICharacterClassSave> ClassesSave { get; init; }

		public IEnumerable<ClassName> ActiveClassesSave { get; init; }
	}
}
