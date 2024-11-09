using Bones.Gameplay.Meta;
using Bones.Gameplay.Meta.CharacterClases;
using Bones.Gameplay.Meta.Currencies;
using Bones.Gameplay.Meta.Equipment;
using UnityEngine;
using Zenject;

namespace Bones.Gameplay.Saving
{
	public class LoadingInitiator : MonoBehaviour
	{
		private ILoader _loader;
		private ICurrencyStorage _currencyStorage;
		private ICharacterClassStorage _characterClassStorage;
		private IEquipmentStorage _equipmentStorage;
		private Level _level;

		[Inject]
		private void Initialize(ILoader loader,
			ICurrencyStorage currencyStorage,
			ICharacterClassStorage characterClassStorage,
			IEquipmentStorage equipmentStorage,
			Level level)
		{
			_loader = loader;
			_currencyStorage = currencyStorage;
			_characterClassStorage = characterClassStorage;
			_equipmentStorage = equipmentStorage;
			_level = level;
		}

		private async void Start()
		{
			ISave save = await _loader.Load();
			_currencyStorage.ApplySave(save.CurrencySave);
			_characterClassStorage.ApplySave(save.CharacterClassStorageSave);
			_equipmentStorage.ApplySave(save.EquipmentStorageSave);
			_level.ApplySave(save.SettingsSave?.CurrentLevel ?? 0, save.SettingsSave?.MaxUnlockedLevel ?? 0);
			Debug.Log($"{save.SettingsSave.CurrentLevel}");
		}
	}
}