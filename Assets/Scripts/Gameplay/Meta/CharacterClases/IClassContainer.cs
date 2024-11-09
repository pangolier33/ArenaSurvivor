using Bones.Gameplay.Spells.Classes;
using System.Collections.Generic;
using UnityEngine;

namespace Bones.Gameplay.Meta.CharacterClases
{
	public interface IClassContainer
	{
		public ClassName Name { get;}
		public Sprite Icon { get;}
		IEnumerable<Skill> GetSkils();
		IEnumerable<ISpellModel> GetModels();
	}
}
