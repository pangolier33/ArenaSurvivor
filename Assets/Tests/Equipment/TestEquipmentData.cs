using Bones.Gameplay.Meta.Equipment;
using Bones.Gameplay.Players;
using Bones.Gameplay.Spells.Classes;
using UnityEngine;

namespace Assets.Tests
{
	internal class TestEquipmentData : IEquipmentData
	{
		public string Id { get; init; }

		public string NameId { get; init; }

		public string DescriptionId { get; init; }

		public EquipmentType Type { get; init; }

		public Sprite Icon { get; init; }

		public ISpellModel SpellModel { get; init; }

		public void ApplyStats(float multiplier, Player player)
		{
			throw new System.NotImplementedException();
		}
	}
}
