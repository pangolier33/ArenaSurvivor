using System;
using UniRx;
using UnityEngine;

namespace Bones.Gameplay.Experience
{
	public class MultiplyingExperienceBank : IExperienceBank
	{
		private readonly IExperienceBank _wrappedBank;

		public MultiplyingExperienceBank(float multiplier, IExperienceBank wrappedBank)
		{
			Multiplier = multiplier;
			_wrappedBank = wrappedBank;
		}

		public float Multiplier { get; set; }
		public void Obtain(float amount) => _wrappedBank.Obtain(Mathf.RoundToInt(amount * Multiplier));
		public void ObtainPercent(float percent) => ObtainPercent(percent, true);
		public void ObtainPercent(float percent, bool amplify) => _wrappedBank.ObtainPercent(percent * (amplify ? Multiplier : 1));

		public IReadOnlyReactiveProperty<int> Level => _wrappedBank.Level;
		public IObservable<IStarModel> Requested => _wrappedBank.Requested;
		public IObservable<IStarModel> Released => _wrappedBank.Released;
	}
}