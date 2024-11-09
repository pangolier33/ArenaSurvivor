using System;
using Bones.Gameplay.Effects.Provider;

namespace Bones.Gameplay.Effects.Pure
{
	public abstract class BoostPEC<T> : PureEffect
	{
		protected sealed override void Invoke(ITrace trace)
		{
			var amplifier = RetrieveAmplifier(trace);
			Apply(amplifier);
			trace.ConnectToClosest(new Booster(amplifier, this));
		}

		protected abstract T RetrieveAmplifier(ITrace trace);
		protected abstract void Apply(T amplifier);
		protected abstract void Dispel(T amplifier);
		
		private class Booster : IDisposable
		{
			private readonly T _amplifier;
			private readonly BoostPEC<T> _origin;

			public Booster(T amplifier, BoostPEC<T> origin)
			{
				_amplifier = amplifier;
				_origin = origin;
			}
			
			public void Dispose()
			{
				_origin.Dispel(_amplifier);
			}
		}
	}
}