using System.Collections.Generic;

namespace Bones.Gameplay.Spells.Classes
{
	public interface ISpellContainer
	{
		IEnumerable<ISpellModel> GetModels();
		IEnumerable<ISpellModel> GetMainModels();
	}
}