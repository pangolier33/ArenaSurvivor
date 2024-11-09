using System;
using System.Collections.Generic;
using System.Linq;
using Bones.Gameplay.Stats.Units.Operations;
using Bones.Gameplay.Stats.Utils;

namespace Bones.Gameplay.Stats.Processors
{
	internal class AmplifierStatProcessor<TStat, TAmplifier> : IStatProcessor<TStat>
		where TAmplifier : struct, IAddSupportable<TAmplifier, TAmplifier>
		where TStat : IMultiplySupportable<TAmplifier, TStat>
	{
		private readonly LinkedList<Func<TAmplifier>> _getters = new();
	
		public TStat Process(TStat from) =>
			from.Multiply(
				_getters.Aggregate(
					default(TAmplifier),
					(amplifier, func) => amplifier.Add(func())
				)
			);

		public IDisposable Add(Func<TAmplifier> getter)
			=> _getters.AddLast(getter).CreateSubscription();
	}
}