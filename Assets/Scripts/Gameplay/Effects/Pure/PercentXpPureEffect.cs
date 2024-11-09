using System;
using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Events.Callbacks.Templates.Base;
using Bones.Gameplay.Experience;
using Zenject;

namespace Bones.Gameplay.Effects.Pure
{
	[Serializable]
	public sealed class PercentXpPureEffect : GetVariablePureEffect<float>, IInjectable
	{
		private IExperienceBank _bank;
		
		protected override void Invoke(ITrace trace, float value)
		{
			_bank.ObtainPercent(value);
		}

		[Inject]
		private void Inject(IExperienceBank bank)
		{
			_bank = bank;
		}
	}
}