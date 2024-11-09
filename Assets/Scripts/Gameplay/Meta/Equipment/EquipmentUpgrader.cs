using Bones.Gameplay.Meta.Currencies;
using System;
using System.Linq;

namespace Bones.Gameplay.Meta.Equipment
{
	public class EquipmentUpgrader
	{
		private readonly IEquipmentStorage _equipmentStorage;
		private readonly ICurrencyStorage _currencyStorage;
		private readonly Currency _silver;

		public EquipmentUpgrader(ICurrencyStorage currencyStorage,
			IEquipmentStorage equipmentStorage)
		{
			_equipmentStorage = equipmentStorage;
			_currencyStorage = currencyStorage;
			_silver = _currencyStorage.Get(CurrencyType.Silver);
		}

		public bool EnoughSilverToMerge(IEquipment mergedEquipment)
		{
			if (mergedEquipment == null)
				throw new ArgumentNullException(nameof(mergedEquipment));

			return mergedEquipment.NeedSilverToMerge <= _silver.Value;
		}

		public void Merge(IEquipment firstEquipment, IEquipment secondEquipment)
		{
			(_equipmentStorage as EquipmentStorage).Merge(firstEquipment, secondEquipment);
			_silver.Withdrow(firstEquipment.NeedSilverToMerge);
		}

		public bool EnoughSilverToUpgradeSlot(EquipmentType type)
		{
			if (type == EquipmentType.None)
				throw new ArgumentNullException(nameof(type));

			var slot = _equipmentStorage.Slots.First(x => x.Type == type);
			return slot.SoftNeedToUpgrade <= _silver.Value;
		}

		public bool EnoughRecipesToUpgradeSlot(EquipmentType type)
		{
			if (type == EquipmentType.None)
				throw new ArgumentNullException(nameof(type));

			var slot = _equipmentStorage.Slots.First(x => x.Type == type);
			var currency = _currencyStorage.Get(type.GetCurrency());
			return slot.RecipesNeedToUpgrade <= currency.Value;
		}

		public void UpgradeSlot(EquipmentType type)
		{
			if (type == EquipmentType.None)
				throw new ArgumentNullException(nameof(type));
			var slot = _equipmentStorage.Slots.First(x => x.Type == type);
			var currency = _currencyStorage.Get(type.GetCurrency());
			(_equipmentStorage as EquipmentStorage).UpgradeSlot(type);
			_silver.Withdrow(slot.SoftNeedToUpgrade);
			currency.Withdrow(slot.RecipesNeedToUpgrade);
		}
	}
}
