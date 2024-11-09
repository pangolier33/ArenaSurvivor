using System;
using Bones.Gameplay.Inputs;
using Bones.Gameplay.Utils;
using UniRx;
using UnityEngine;

namespace Bones.Gameplay.Experience
{
	public class ReleasingExperienceBank : IExperienceBank, IDisposable
	{
		private readonly IDisposable _trackerSubscription;
		private readonly ExperienceBank _wrappedBank;

		public ReleasingExperienceBank(InputObservable input, int limit, ExperienceBank wrappedBank)
		{
			Limit = limit;
			_wrappedBank = wrappedBank;
			_trackerSubscription = input.Direction.Subscribe(OnPositionUpdated);
		}

		public int Limit { get; }
		public IReadOnlyReactiveProperty<int> Level => _wrappedBank.Level;

		public IObservable<IStarModel> Requested => _wrappedBank.Requested;
		public IObservable<IStarModel> Released => _wrappedBank.Released;

		private bool ShouldRelease => _wrappedBank.StarsCount >= Limit && _wrappedBank.IsAllFull;

		public void Obtain(float amount)
		{
			if (ShouldRelease)
				_wrappedBank.Release();
			else
				_wrappedBank.Obtain(amount);
		}

		public void ObtainPercent(float percent)
		{
			if (ShouldRelease)
				_wrappedBank.Release();
			else _wrappedBank.ObtainPercent(percent);
		}

		public void Dispose()
		{
			_trackerSubscription.Dispose();
		}

		private void OnPositionUpdated(Vector2 delta)
		{
			if (!delta.Approximately(Vector2.zero))
				return;
            
			_wrappedBank.Release();
		}
	}
}