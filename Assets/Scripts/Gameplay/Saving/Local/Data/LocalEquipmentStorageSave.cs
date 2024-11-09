using Bones.Gameplay.Meta.Equipment;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bones.Gameplay.Saving.Local
{
	[Serializable]
	public class LocalEquipmentStorageSave : IEquipmentStorageSave
	{
		public LocalEquipmentSave[] _euiped;
		public LocalEquipmentSave[] _backpack;
		public LocalEquipmentSlotSave[] _slots;

		public IEnumerable<IEquipmentSave> Euiped { get => _euiped; set => _euiped = value.Cast<LocalEquipmentSave>().ToArray(); }

		public IEnumerable<IEquipmentSave> Backpack { get => _backpack; set => _backpack = value.Cast<LocalEquipmentSave>().ToArray(); }

		public IEnumerable<IEquipmentSlotSave> Slots { get => _slots; set => _slots = value.Cast<LocalEquipmentSlotSave>().ToArray(); }
	}
}
