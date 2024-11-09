using Bones.Gameplay.Meta.CharacterClases;
using Bones.Gameplay.Meta.Currencies;
using Bones.Gameplay.Meta.Equipment;
using UnityEngine;
using Zenject;

namespace Bones.Gameplay.Saving
{
	public class StorageTest : MonoBehaviour
	{
		private ICurrencyStorage _currencyStorage;
		private ICharacterClassStorage _characterClassStorage;
		private IEquipmentStorage _equipmentStorage;

		[Inject]
		public void Initialize(ICurrencyStorage currencyStorage,
			ICharacterClassStorage characterClassStorage,
			IEquipmentStorage equipmentStorage)
		{
			_currencyStorage = currencyStorage;
			_characterClassStorage = characterClassStorage;
			_equipmentStorage = equipmentStorage;
			_currencyStorage.Get(CurrencyType.Crystal).Deposit(10);
		}
	}
}