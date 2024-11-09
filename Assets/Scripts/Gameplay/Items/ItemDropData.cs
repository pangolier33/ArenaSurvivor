using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Bones.Gameplay.Items
{
	[Serializable]
	public class ItemDropData
	{
		[ShowInInspector, PropertyOrder(-1), ReadOnly, PreviewField(ObjectFieldAlignment.Center), TableColumnWidth(70, false)]
		public GameObject Preview => ItemPrefab != null ? ItemPrefab.gameObject : null;
		[Required, TableColumnWidth(170, false)]
		public Item ItemPrefab;

		[PropertyRange(0, 100)] public float Chance = 1;
		public Vector2Int Count = Vector2Int.one;

		protected string GetName() => $"{ItemPrefab?.name} | {Chance} | {(Count.x == Count.y ? Count.x : Count)}";
	}
}