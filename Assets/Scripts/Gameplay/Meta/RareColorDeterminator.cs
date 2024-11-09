using Bones.Gameplay.Meta.Equipment;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Bones.Gameplay.Meta
{
	public static class RareColorDeterminator
	{
		private static RareColor[] _rareColors;

		public static void Initialize(IEnumerable<RareColor> rareColors)
		{
			_rareColors = rareColors.ToArray();
		}

		public static Color GetColor(Rare rare)
		{
			if (_rareColors == null)
				throw new InvalidOperationException("Rare colors not initialized");

			return _rareColors.First(x => x.Rare == rare).Color;
		}
	}

	[Serializable]
	public class RareColor
	{
		[SerializeField] private Rare _rare;
		[SerializeField] private Color _color;

		public Rare Rare => _rare;
		public Color Color => _color;
	}
}
