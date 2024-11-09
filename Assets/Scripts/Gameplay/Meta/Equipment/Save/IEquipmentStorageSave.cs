using System.Collections.Generic;

namespace Bones.Gameplay.Meta.Equipment
{
	public interface IEquipmentStorageSave
	{
		IEnumerable<IEquipmentSave> Euiped { get; }
		IEnumerable<IEquipmentSave> Backpack { get; }
		IEnumerable<IEquipmentSlotSave> Slots { get; }
	}
}
