using System;
using System.Collections.Generic;
using System.Linq;
using Bones.Gameplay.Stats.Containers;
using Bones.Gameplay.Stats.Processors;
using Bones.Gameplay.Stats.Units.Operations;

namespace Bones.Gameplay
{
	public sealed class SummarisingStatWrapper<T> : IGetStat<T> where T : IAddSupportable<T, T>
	{
		ref IStatProcessor<T> IGetStat<T>.Processor => throw new NotSupportedException();
		public IEnumerable<IGetStat<T>> WrappedStats { get; init; }
		public T Get() => WrappedStats.Aggregate(default(T), (value, stat) => value.Add(stat.Get()));

		public override string ToString() => $"SummarisingStatWrapper [{string.Join(",", WrappedStats)}]";
	}
}