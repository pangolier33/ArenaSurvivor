using Bones.Gameplay.Meta.Currencies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bones.Gameplay.Meta.Equipment
{
	public static class EquipmentTypeExtensions
	{
		public static CurrencyType GetCurrency(this EquipmentType equipmentType) 
		{
			switch (equipmentType)
			{
				case EquipmentType.Armor:
					return CurrencyType.ArmorRecipe;
				case EquipmentType.Belt:
					return CurrencyType.BeltRecipe;
				case EquipmentType.Glove:
					return CurrencyType.GloveRecipe;
				case EquipmentType.Boots:
					return CurrencyType.BootsRecipe;
				default:
					throw new InvalidCastException($"Unsupported equipment type {equipmentType}");
			}
		}
	}
}
