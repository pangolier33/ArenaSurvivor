using System;
using System.Collections.Generic;
using System.Linq;
using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Spells.Classes;
using Bones.Gameplay.Stats.Containers.Graphs;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Bones.Gameplay.Stats.Containers.Builders
{
	public abstract class DictionaryStatsConfiguration : IFactory<IStatMap>
	{
		public IStatMap Create() => new Adapter(ToDictionary());
		protected abstract IDictionary<StatName, IStat> ToDictionary();

		public class Adapter : IStatMap
		{
			private readonly IDictionary<StatName, IStat> _dictionary;
			public Adapter(IDictionary<StatName, IStat> dictionary) => _dictionary = dictionary;
			public IEnumerable<KeyValuePair<StatName, IStat>> GetAll() => _dictionary;
			public bool TryGet(StatName name, out IStat stat) => _dictionary.TryGetValue(name, out stat);
			public IStat Get(StatName name) => _dictionary[name];
			public bool Contains(StatName name) => _dictionary.ContainsKey(name);

			public override string ToString()
			{
				return $"Dictionary StatMap:\n{string.Join(",\n", _dictionary.Select(x => $"{x.Key}: {x.Value}}}"))}";
			}
		}

		[Serializable]
		public struct StatPair : IStatInfo
		{
			[field: SerializeField, TableColumnWidth(70, Resizable = false)] public bool Overwrite { get; private set; }
			[field: SerializeField, TableColumnWidth(150, Resizable = false)] public StatName Name { get; private set; }
			[field: SerializeReference, LabelText(" "), LabelWidth(15), InlineProperty, HideReferenceObjectPicker]
			public IStatBuilder Builder { get; private set; }
		}
	}
}