using Bones.Gameplay.Players;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Bones.Gameplay.Meta.Equipment
{
	public class Equiper : MonoBehaviour
	{
		[Inject]
		private void Initialize(Player player,
			IEquipmentStorage equipmentStorage)
		{
			//foreach (var equpment in equipmentStorage.AllEquipment)
			//equipmentStorage.AddToBackpack(equpment.Id, Rare.Rare, 2);

			//equipmentStorage.Equip(equipmentStorage.Backpack.First());

			foreach (var equiped in equipmentStorage.Equiped)
				equiped.Apply(player);
		}
	}
}
