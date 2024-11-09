using System;
using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Events.Callbacks.Templates.Base;
using Bones.Gameplay.Experience;
using Bones.Gameplay.Stats.Units;
using Zenject;

namespace Bones.Gameplay.Effects.Pure
{
	[Serializable]
	public class BoostExperienceAmplifierPEC : GetStatPureEffect<Value>, IInjectable
	{
		private MultiplyingExperienceBank _bank;
		
		protected override void Invoke(ITrace trace, Value value)
		{
			trace.ConnectToClosest(new Booster(value, _bank));
		}

		[Inject]
		private void Inject(MultiplyingExperienceBank bank)
		{
			_bank = bank;
		}

		private class Booster : IDisposable
		{
			private readonly float _multiplier;
			private readonly MultiplyingExperienceBank _bank;

			public Booster(float multiplier, MultiplyingExperienceBank bank)
			{
				_multiplier = multiplier;
				_bank = bank;
				_bank.Multiplier += _multiplier;
			}
			
			public void Dispose()
			{
				_bank.Multiplier -= _multiplier;
			}
		}
	}
}