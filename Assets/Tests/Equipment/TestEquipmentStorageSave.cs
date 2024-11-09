using Bones.Gameplay.Meta.Equipment;
using System.Collections.Generic;

namespace Assets.Tests
{
	internal class TestEquipmentStorageSave : IEquipmentStorageSave
	{
		public IEnumerable<IEquipmentSave> Euiped { get; init; }

		public IEnumerable<IEquipmentSave> Backpack { get; init; }

		public IEnumerable<IEquipmentSlotSave> Slots { get; init; }
	}
}
