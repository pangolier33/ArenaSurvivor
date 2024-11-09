using Bones.Gameplay.Players;

namespace Bones.Gameplay.Meta.Equipment
{
	public interface IEquipment
	{
		public const int MAX_RARE_LEVEL = 3;

		string Id { get; }
		EquipmentType Type { get; }
		Rare Rare { get; }
		int RareLevel { get; }
		float NeedSilverToMerge { get; }
		float RareMultiplier { get; }
		int Level { get; }
		float LevelMultiplier { get; }
		int Livetime { get; }
		int Durability { get; }
		IEquipmentData Data { get; }
		void Apply(Player player);
	}
}
