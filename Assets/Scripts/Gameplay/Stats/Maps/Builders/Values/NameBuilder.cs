using System;
using Bones.Gameplay.Effects.Transitive.Depth;
using Bones.Gameplay.Stats.Containers;
using Bones.Gameplay.Stats.Containers.Graphs;
using UnityEngine;

namespace Bones.Gameplay
{
	public class NameBuilder : IStatBuilder
	{
		[SerializeField] private StatName _name;
		
		public IStat Build() => new ReadOnlyStat<StatName>(_name);
		public IStat BuildUncertain() => throw new NotSupportedException();
		public IStat BuildWrapper(IStat wrappedStat) => Build();
	}
}