using System;
using Railcar.Time.Subscriptions;

namespace Railcar.Time
{
	public class TimeFacade : IDisposable
	{
		private readonly Timer _timer = new();
		private readonly Stopwatch _stopwatch = new();
		private readonly Clock _clock = new();

		public ITimer Timer => _timer;
		public IStopwatch Stopwatch => _stopwatch;
		public IClock Clock => _clock;
		public IObservableClock ObservableClock => _clock;

		public void Update(float timeDelta)
		{
			_clock.Update(timeDelta);
			_stopwatch.Update(timeDelta);
			_timer.Update(_clock.Current);
		}

		public void Dispose()
		{
			_timer.Dispose();
			_stopwatch.Dispose();
			_clock.Dispose();
		}
	}
}