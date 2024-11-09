using Bones.Gameplay.Spells.Classes;
using System.Collections.Generic;

namespace Bones.Gameplay.Meta.CharacterClases
{
	public interface ICharacterClassStorageSave
	{
		IEnumerable<ICharacterClassSave> ClassesSave { get; }
		IEnumerable<ClassName> ActiveClassesSave { get; }
	}
}
