using System;
using Bones.Gameplay.Stats.Containers;
using Bones.Gameplay.Stats.Processors;
using Bones.Gameplay.Stats.Units.Operations;

namespace Bones.Gameplay
{
	public class StatWrapper<TSource, TAmplifier> : IGetStat<TSource>
		where TSource : IMultiplySupportable<TAmplifier, TSource>
	{
		private readonly IGetStat<TSource> _wrappedStat;
		private readonly TAmplifier _amplifier;

		public StatWrapper(IGetStat<TSource> wrappedStat, TAmplifier amplifier)
		{
			_wrappedStat = wrappedStat;
			_amplifier = amplifier;
		}

		public TSource Get() => _wrappedStat.Get().Multiply(_amplifier);
		ref IStatProcessor<TSource> IGetStat<TSource>.Processor => throw new NotSupportedException();
	}
}