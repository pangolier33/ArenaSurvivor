using System;
using System.Collections.Generic;
using System.Linq;
using Bones.Gameplay.Stats.Containers;
using Bones.Gameplay.Stats.Containers.Builders;
using Bones.Gameplay.Stats.Containers.Graphs;
using UnityEngine;

namespace Bones.Gameplay
{
	[Serializable]
	public sealed class SerializableCustomStatsConfiguration : DictionaryStatsConfiguration
	{
		[SerializeField] private List<StatPair> _builders;

		protected override IDictionary<StatName, IStat> ToDictionary() =>
			_builders.ToDictionary(x => x.Name, x => x.Builder.Build());
	}
}