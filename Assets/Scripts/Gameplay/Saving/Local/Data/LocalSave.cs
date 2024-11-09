using Bones.Gameplay.Meta;
using Bones.Gameplay.Meta.CharacterClases;
using Bones.Gameplay.Meta.Currencies;
using Bones.Gameplay.Meta.Equipment;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bones.Gameplay.Saving.Local
{
	[Serializable]
	public class LocalSave : ISave
	{
		public LocalCurrencySave[] _currencySaveLocal;
		public LocalCharacterClassStorageSave _characterClassSaveLocal;
		public LocalEquipmentStorageSave _equipmentSaveLocal;
		public LocalSettingsSave _settingsSave;

		public IEnumerable<ICurrencySave> CurrencySave
		{
			get { return _currencySaveLocal; }
			set { _currencySaveLocal = value.Cast<LocalCurrencySave>().ToArray(); }
		}

		public ICharacterClassStorageSave CharacterClassStorageSave
		{
			get { return _characterClassSaveLocal; }
			set { _characterClassSaveLocal = value as LocalCharacterClassStorageSave; }
		}

		public IEquipmentStorageSave EquipmentStorageSave
		{
			get { return _equipmentSaveLocal; }
			set { _equipmentSaveLocal = value as LocalEquipmentStorageSave; }
		}

		public ISettingsSave SettingsSave { get => _settingsSave; set => _settingsSave = (LocalSettingsSave) value; }
	}
}
