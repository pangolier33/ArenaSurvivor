using Bones.Gameplay.Meta.Equipment;
using System;

namespace Bones.Gameplay.Saving.Local
{
	[Serializable]
	public class LocalEquipmentSave : IEquipmentSave
	{
		public string _id;
		public Rare _rare;
		public int _rareLevel;

		public string Id { get => _id; set => _id = value; }

		public Rare Rare { get => _rare; set => _rare = value; }

		public int RareLevel { get => _rareLevel; set => _rareLevel = value; }
	}
}
