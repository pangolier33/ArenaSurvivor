using System;

namespace Railcar.Time
{
	public interface IStopwatch
	{
		IDisposable Observe(Action<float> callback);
	}
}