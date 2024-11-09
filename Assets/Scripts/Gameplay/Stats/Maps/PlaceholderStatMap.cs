using System;
using System.Collections.Generic;
using System.Linq;
using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Stats.Containers;
using Bones.Gameplay.Stats.Containers.Graphs;

namespace Bones.Gameplay.Spells.Classes
{
	public class PlaceholderStatMap : IStatMap
	{
		private static PlaceholderStatMap s_instance;
		private PlaceholderStatMap() {}
		public static IStatMap Instance => s_instance ??= new();
		public IEnumerable<KeyValuePair<StatName, IStat>> GetAll() => Enumerable.Empty<KeyValuePair<StatName, IStat>>();
		public bool TryGet(StatName name, out IStat stat)
		{
			stat = null;
			return false;
		}
		public IStat Get(StatName name) => throw new NullReferenceException();
		public bool Contains(StatName name) => false;
		public override string ToString()
		{
			return "Placeholder StatMap";
		}
	}
}