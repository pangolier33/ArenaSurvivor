using Bones.Gameplay.Meta;
using Bones.Gameplay.Meta.Currencies;
using Bones.Gameplay.Meta.Equipment;
using Bones.Gameplay.Spells.Classes;
using System.Threading.Tasks;
using UnityEngine;

namespace Bones.Gameplay.Saving.Local
{
	public class LocalLoader : ILoader
	{
		public const string LOCAL_SAVE = nameof(LOCAL_SAVE);

		public bool Loaded { get; set; }

		public Task<ISave> Load()
		{
			Loaded = true;
			//if (PlayerPrefs.HasKey(LOCAL_SAVE))
			//{
			//	string json = PlayerPrefs.GetString(LOCAL_SAVE);
			//	ISave save = JsonUtility.FromJson<LocalSave>(json);
			//	return Task.FromResult(save);
			//}
			//else
			{
				return Task.FromResult(GetFakeSave());
			}
		}

		private static ISave GetFakeSave()
		{
			return new LocalSave()
			{
				CurrencySave = new LocalCurrencySave[]
			{
					new LocalCurrencySave() { Type = CurrencyType.Silver, Value = 100},
					new LocalCurrencySave() { Type= CurrencyType.Crystal, Value = 10 }
			},
				SettingsSave = new LocalSettingsSave() { CurrentLevel = 0, MaxUnlockedLevel = 0 },
				CharacterClassStorageSave = new LocalCharacterClassStorageSave()
				{
					ClassesSave = new LocalCharacterClassSave[]
					{
						new LocalCharacterClassSave() {Copies = 0, Name = ClassName.Warrior, Rare = Rare.Common, Experience = 0, Level = 1},
						new LocalCharacterClassSave() {Copies = 0, Name = ClassName.Healer, Rare = Rare.Common, Experience = 0, Level = 1},
						new LocalCharacterClassSave() {Copies = 0, Name = ClassName.Mage, Rare = Rare.Common, Experience = 0, Level = 1},
					},
					ActiveClassesSave = new ClassName[] { ClassName.Warrior, ClassName.Healer, ClassName.Mage, ClassName.Defender },
				},
				EquipmentStorageSave = new LocalEquipmentStorageSave()
				{
					Backpack = new LocalEquipmentSave[]
					{
						new LocalEquipmentSave() {Id = "herbal_glove", Rare = Rare.Common, RareLevel = 3},
						new LocalEquipmentSave() {Id = "herbal_boots", Rare = Rare.Rare, RareLevel = 2},
						new LocalEquipmentSave() {Id = "herbal_armor", Rare = Rare.Magical, RareLevel = 1},
						new LocalEquipmentSave() {Id = "herbal_belt", Rare = Rare.Legendary, RareLevel = 1},
						new LocalEquipmentSave() {Id = "fire_armor", Rare = Rare.Common, RareLevel = 1},
						new LocalEquipmentSave() {Id = "fire_belt", Rare = Rare.Rare, RareLevel = 2},
						new LocalEquipmentSave() {Id = "fire_boots", Rare = Rare.Magical, RareLevel = 3},
						new LocalEquipmentSave() {Id = "fire_gloves", Rare = Rare.Legendary, RareLevel = 2},
					},
					Euiped = new LocalEquipmentSave[]
					{
					}
				}
			};
		}
	}
}
