using Bones.Gameplay.Stats.Containers.Graphs;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Bones.Gameplay.Meta.Equipment
{
	[Serializable]
	public struct StatRow
	{
		[field: SerializeField, TableColumnWidth(70, Resizable = false)] 
		public bool Multiply { get; private set; }

		[field: SerializeField, TableColumnWidth(150, Resizable = false)] 
		public StatName Stat { get; private set; }

		[field: SerializeReference]
		public float Value { get; private set; }
	}
}
