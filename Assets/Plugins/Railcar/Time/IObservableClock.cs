using UniRx;

namespace Railcar.Time
{
	public interface IObservableClock
	{
		IReadOnlyReactiveProperty<float> Current { get; }
	}
}