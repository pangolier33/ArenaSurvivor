using System;
using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Events.Callbacks.Templates.Base;
using Bones.Gameplay.Experience;
using Zenject;

namespace Bones.Gameplay.Effects.Pure
{
	[Serializable]
	public sealed class XpPureEffect : GetVariablePureEffect<int>, IInjectable
	{
		private IExperienceBank _bank;
		
		protected override void Invoke(ITrace trace, int value)
		{
			_bank.Obtain(value);
		}

		[Inject]
		private void Inject(IExperienceBank bank)
		{
			_bank = bank;
		}
	}
}