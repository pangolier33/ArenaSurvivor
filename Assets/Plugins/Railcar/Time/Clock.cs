using System;
using UniRx;

namespace Railcar.Time
{
	public class Clock : IObservableClock, IClock, IDisposable
	{
		private readonly ReactiveProperty<float> _current = new();

		IReadOnlyReactiveProperty<float> IObservableClock.Current => _current;
		public float Current => _current.Value;

		public void Update(float delta) => _current.Value += delta;
		public void Dispose() => _current.Dispose();
	}
}