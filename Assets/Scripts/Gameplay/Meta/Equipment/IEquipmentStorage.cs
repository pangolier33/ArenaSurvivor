using System;
using System.Collections.Generic;

namespace Bones.Gameplay.Meta.Equipment
{
	public interface IEquipmentStorage
	{
		IEnumerable<IEquipment> AllEquipment { get; }
		IEnumerable<IEquipment> Backpack { get; }
		IEnumerable<IEquipment> Equiped { get; }
		IEnumerable<IEquipmentSlot> Slots { get; }

		event Action<IEnumerable<IEquipment>> BackpackChanged;
		event Action<IEnumerable<IEquipment>> EquipedChanged;
		event Action<IEnumerable<IEquipmentSlot>> SlotsChanged;

		void ApplySave(IEquipmentStorageSave save);
		void AddToBackpack(string id, Rare rare, int rareLevel);
		void Equip(IEquipment equipment);
		bool IsEquiped(EquipmentType type);
		void Unequip(IEquipment equipment);
	}
}