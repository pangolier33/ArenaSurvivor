using Bones.Gameplay.Meta;
using Bones.Gameplay.Meta.Currencies;
using Bones.Gameplay.Saving.Local;
using Bones.Gameplay.Spells.Classes;
using System.Linq;

namespace Bones.Gameplay.Saving.UnityCloud
{
	public class UnityCloudSaveCreator : ISaveCreator
	{
		private readonly ICurrencyStorage _currencyStorage;
		private readonly Level _level;

		public UnityCloudSaveCreator(ICurrencyStorage currencyStorage,
			Level level)
		{
			_currencyStorage = currencyStorage;
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
					ClassesSave = new LocalCharacterClassSave[] { },
					ActiveClassesSave = new ClassName[0],
				},
				EquipmentStorageSave = new LocalEquipmentStorageSave()
				{
					Backpack = new LocalEquipmentSave[] { },
					Euiped = new LocalEquipmentSave[] { }
				}
			};
		}
	}
}
