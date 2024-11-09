using System;
using System.Collections.Generic;
using System.Linq;
using Bones.Gameplay.Stats.Containers.Graphs;
using UnityEngine;

namespace Bones.Gameplay.Spells.Classes
{
	[Serializable]
	public class StatLineFormatter
	{
		[SerializeField] private List<Pair> _icons;

		public string Format(StatName name, object value) => string.Format(_icons.First(x => x.Name == name).Formatter, value);
		public string GetIcon(StatName name) => _icons.FirstOrDefault(x => x.Name == name).Icon ?? throw new ArgumentOutOfRangeException(nameof(name), name, "Icon for name is not provided"); 
        
		[Serializable]
		private struct Pair
		{
			[field: SerializeField] public StatName Name { get; private set; }
			[field: SerializeField] public string Formatter { get; private set; }
			[field: SerializeField] public string Icon { get; private set; }
		}
	}
}