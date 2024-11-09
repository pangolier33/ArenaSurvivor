using System;
using Railcar.Time;
using Zenject;

namespace Bones.UI.Presenters
{
	public class TimePresenter : InjectedPresenter<float>
	{
		private IObservableClock _clock;
		
		protected override IObservable<float> RetrieveModel()
		{
			return _clock.Current;
		}

		[Inject]
		private void Inject([Inject(Id = TimeID.Fixed)] IObservableClock time)
		{
			_clock = time;
		}
	}
}