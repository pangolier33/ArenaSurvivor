namespace Bones.Gameplay.Meta.Equipment
{
	public class EquipmentSlot : IEquipmentSlot
	{
		public EquipmentType Type { get; }
		public int Level { get; private set; }
		public int SoftNeedToUpgrade => SoftToSlotUpgradeCalculator.NeedToUpgrade(Level);

		public int RecipesNeedToUpgrade => RecipesToSlotUpgradeCalculator.NeedToUpgrade(Level);

		public EquipmentSlot(EquipmentType type, int level)
		{
			Type = type;
			Level = level;
		}

		public void Upgrade()
		{
			Level++;
		}
	}
}
