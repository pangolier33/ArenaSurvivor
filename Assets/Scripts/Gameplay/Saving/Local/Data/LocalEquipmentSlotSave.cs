using Bones.Gameplay.Meta.Equipment;
using System;

namespace Bones.Gameplay.Saving.Local
{
	[Serializable]
	public class LocalEquipmentSlotSave : IEquipmentSlotSave
	{
		public EquipmentType _type;
		public int _level;

		public EquipmentType Type { get { return _type; } set { _type = value; } }
		public int Level { get { return _level; } set { _level = value; } }
	}
}
