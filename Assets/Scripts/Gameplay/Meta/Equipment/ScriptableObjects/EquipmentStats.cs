using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Bones.Gameplay.Meta.Equipment
{
	[CreateAssetMenu(menuName = "Data/EquipmentStats")]
	public class EquipmentStats : ScriptableObject
	{
		[TableList(AlwaysExpanded = true, ShowPaging = false, DefaultMinColumnWidth = 50)]
		[SerializeField] private List<StatRow> _stats;
	}
}
