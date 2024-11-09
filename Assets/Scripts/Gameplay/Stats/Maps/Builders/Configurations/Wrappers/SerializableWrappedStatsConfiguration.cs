using System;
using System.Collections.Generic;
using System.Linq;
using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Stats.Containers.Graphs;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Bones.Gameplay.Stats.Containers.Builders.Wrappers
{
	[Serializable]
	public sealed class SerializableWrappedStatsConfiguration : DictionaryStatsConfiguration
	{
		[TableList(AlwaysExpanded = true,ShowPaging = false,DefaultMinColumnWidth = 50)]
		[SerializeField] private List<StatPair> _pairs;
		private IMutableTrace _trace;

		protected override IDictionary<StatName, IStat> ToDictionary()
		{
			return _pairs.ToDictionary(x => x.Name, x => x.Overwrite ? x.Builder.Build() : x.Builder.BuildWrapper(_trace.GetStat(x.Name)));
		}

		[Inject]
		private void Inject(IMutableTrace trace)
		{
			_trace = trace;
		}
	}
}