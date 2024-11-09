using Bones.Gameplay.Players;
using Bones.Gameplay.Spells.Classes;
using UnityEngine;

namespace Bones.Gameplay.Meta.Equipment
{
	public interface IEquipmentData
	{
		public string Id { get; }
		public string NameId { get; }
		public string DescriptionId { get; }
		public EquipmentType Type { get; }
		public Sprite Icon { get; }
		public ISpellModel SpellModel { get; }

		public void ApplyStats(float multiplier, Player player);
	}
}
