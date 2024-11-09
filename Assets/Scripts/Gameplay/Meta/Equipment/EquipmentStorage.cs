using System;
using System.Collections.Generic;
using System.Linq;

namespace Bones.Gameplay.Meta.Equipment
{
	public class EquipmentStorage : IEquipmentStorage
	{
		public const int MERGE_COUNT = 3;

		private List<IEquipment> _backpack = new();
		private List<IEquipment> _equiped = new();
		private List<IEquipmentSlot> _slots = new();

		public EquipmentStorage(IEnumerable<IEquipmentData> allEquipment)
		{
			AllEquipment = allEquipment.Select(x => new Equipment(x)).ToArray();
		}

		public void ApplySave(IEquipmentStorageSave save)
		{
			FillEquipments(save.Backpack, _backpack);
			FillEquipments(save.Euiped, _equiped);
			FillSlots(save.Slots);
			foreach (var slot in Slots)
				UpdateEquipmentsLevel(slot);

			BackpackChanged?.Invoke(Backpack);
		}

		private void FillEquipments(IEnumerable<IEquipmentSave> save, List<IEquipment> storage)
		{
			foreach (var equipmentSave in save)
			{
				var original = AllEquipment.FirstOrDefault(x => x.Id == equipmentSave.Id);
				if (original == null)
					throw new InvalidOperationException($"Can't find original equipment from save {equipmentSave.Id}");
				storage.Add(new Equipment(original, equipmentSave.Rare, equipmentSave.RareLevel));

			}
		}

		private void FillSlots(IEnumerable<IEquipmentSlotSave> slots)
		{
			if (slots == null || slots.Count() == 0)
			{
				_slots.Add(new EquipmentSlot(EquipmentType.Armor, 0));
				_slots.Add(new EquipmentSlot(EquipmentType.Belt, 0));
				_slots.Add(new EquipmentSlot(EquipmentType.Boots, 0));
				_slots.Add(new EquipmentSlot(EquipmentType.Glove, 0));
			}
			else
			{
				foreach (var slot in slots)
					_slots.Add(new EquipmentSlot(slot.Type, slot.Level));
			}
		}

		private void UpdateEquipmentsLevel(IEquipmentSlot slot)
		{
			var filteredEquipments = Backpack.Concat(Equiped)
				.Where(x => x.Type == slot.Type);
			foreach (var equipment in filteredEquipments)
				(equipment as Equipment).Level = slot.Level;
		}

		public IEnumerable<IEquipment> AllEquipment { get; }
		public IEnumerable<IEquipment> Backpack => _backpack.ToList();
		public IEnumerable<IEquipment> Equiped => _equiped.ToList();
		public IEnumerable<IEquipmentSlot> Slots => _slots.ToList();

		public event Action<IEnumerable<IEquipment>> BackpackChanged;
		public event Action<IEnumerable<IEquipment>> EquipedChanged;
		public event Action<IEnumerable<IEquipmentSlot>> SlotsChanged;

		public void Equip(IEquipment equipment)
		{
			if (equipment == null)
				throw new ArgumentNullException(nameof(equipment));
			if (!Backpack.Contains(equipment))
				throw new InvalidOperationException($"Backpack don't have {equipment.Id}");

			var previousEquiped = Equiped.FirstOrDefault(x => x.Type == equipment.Type);
			if (previousEquiped != null)
			{
				_equiped.Remove(previousEquiped);
				_backpack.Add(previousEquiped);
			}
			_equiped.Add(equipment);
			_backpack.Remove(equipment);

			BackpackChanged?.Invoke(Backpack);
			EquipedChanged?.Invoke(Equiped);
		}

		public bool IsEquiped(EquipmentType type)
		{
			return Equiped.Any(x => x.Type == type);
		}

		public void Unequip(IEquipment equipment)
		{
			if (equipment == null)
				throw new ArgumentNullException(nameof(equipment));
			if (!Equiped.Contains(equipment))
				throw new InvalidOperationException($"{equipment.Id} don't equiped.");

			_backpack.Add(equipment);
			_equiped.Remove(equipment);

			BackpackChanged?.Invoke(Backpack);
			EquipedChanged?.Invoke(Equiped);
		}

		public void AddToBackpack(string id, Rare rare, int rareLevel)
		{
			if (string.IsNullOrWhiteSpace(id))
				throw new ArgumentNullException(nameof(id));
			var equipment = AllEquipment.FirstOrDefault(x => x.Id == id);
			if (equipment == null)
				throw new InvalidOperationException($"Unknown equipment {id}");
			var newEquipment = new Equipment(equipment, rare, rareLevel);
			newEquipment.Level = Slots.First(x => x.Type == newEquipment.Type).Level;
			_backpack.Add(newEquipment);

			BackpackChanged?.Invoke(Backpack);
		}

		public void Merge(IEquipment firstEquipment, IEquipment secondEquipment)
		{
			if (firstEquipment == null)
				throw new ArgumentNullException(nameof(firstEquipment));
			if (secondEquipment == null)
				throw new ArgumentNullException (nameof(secondEquipment));

			if (firstEquipment.Id != secondEquipment.Id)
				throw new InvalidOperationException($"Can't merge equipment {firstEquipment.Id} with {secondEquipment.Id}.");
			if (!Backpack.Contains(firstEquipment))
				throw new InvalidOperationException($"Backpack don't have {firstEquipment.Id}.");
			if (!Backpack.Contains(secondEquipment)) 
				throw new InvalidOperationException($"Backpack don't have {secondEquipment.Id}");

			_backpack.Remove(secondEquipment);
			((Equipment)firstEquipment).IncreaseRare();

			BackpackChanged?.Invoke(Backpack);
		}

		public void UpgradeSlot(EquipmentType type)
		{
			if (type == EquipmentType.None)
				throw new ArgumentNullException(nameof(type));

			var slot = Slots.First(x => x.Type == type);
			(slot as EquipmentSlot).Upgrade();
			UpdateEquipmentsLevel(slot);
			SlotsChanged?.Invoke(Slots);
		}
	}
}
