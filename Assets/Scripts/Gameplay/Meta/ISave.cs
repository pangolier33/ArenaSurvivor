using Bones.Gameplay.Meta.CharacterClases;
using Bones.Gameplay.Meta.Currencies;
using Bones.Gameplay.Meta.Equipment;
using Bones.Gameplay.Saving;
using System.Collections.Generic;

namespace Bones.Gameplay.Meta
{
	public interface ISave
	{
		IEnumerable<ICurrencySave> CurrencySave { get; }
		ICharacterClassStorageSave CharacterClassStorageSave { get; }
		IEquipmentStorageSave EquipmentStorageSave { get; }
		ISettingsSave SettingsSave { get; }
	}
}
