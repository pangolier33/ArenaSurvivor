using System.Collections.Generic;
using UnityEngine;

namespace Bones.Gameplay.Spells.Classes
{
	public abstract class BaseSpellContainer : ScriptableObject, ISpellContainer
	{
		public abstract IEnumerable<ISpellModel> GetMainModels();

		public abstract IEnumerable<ISpellModel> GetModels();
	}
}