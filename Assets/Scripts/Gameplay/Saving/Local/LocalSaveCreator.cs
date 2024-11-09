using Bones.Gameplay.Meta;
using Bones.Gameplay.Meta.CharacterClases;
using Bones.Gameplay.Meta.Currencies;
using Bones.Gameplay.Meta.Equipment;
using System.Linq;

namespace Bones.Gameplay.Saving.Local
{
	public class LocalSaveCreator : ISaveCreator
	{
		private ICurrencyStorage _currencyStorage;
		private IEquipmentStorage _equipmentStorage;
		private ICharacterClassStorage _characterClassStorage;
		private Level _level;

		public LocalSaveCreator(ICurrencyStorage currencyStorage,
			IEquipmentStorage equipmentStorage,
			ICharacterClassStorage characterClassStorage,
			Level level)
		{
			_currencyStorage = currencyStorage;
			_equipmentStorage = equipmentStorage;
			_characterClassStorage = characterClassStorage;
			_level = level;
		}

		public ISave Create()
		{
			return new LocalSave()
			{
				CurrencySave = _currencyStorage.Select(x => new LocalCurrencySave() { Type = x.Type, Value = x.Value }),
				SettingsSave = new LocalSettingsSave()
				{
					CurrentLevel = _level.Current.Value,
					MaxUnlockedLevel = _level.MaxUnlocked.Value
				},
				CharacterClassStorageSave = new LocalCharacterClassStorageSave()
				{
					ClassesSave = _characterClassStorage.AllClasses.Select(x => new LocalCharacterClassSave(x)),
					ActiveClassesSave = _characterClassStorage.ActiveClasses.Select(x => x.Name)
				},
				EquipmentStorageSave = new LocalEquipmentStorageSave()
				{
					Backpack = _equipmentStorage.Backpack.Select(x => new LocalEquipmentSave()
					{
						Id = x.Id,
						Rare = x.Rare,
						RareLevel = x.RareLevel
					}),
					Euiped = _equipmentStorage.Equiped.Select(x => new LocalEquipmentSave()
					{
						Id = x.Id,
						Rare = x.Rare,
						RareLevel = x.RareLevel
					}),
					Slots = _equipmentStorage.Slots.Select(x => new LocalEquipmentSlotSave()
					{
						Type = x.Type,
						Level = x.Level,
					})
				}
			};
		}
	}
}
