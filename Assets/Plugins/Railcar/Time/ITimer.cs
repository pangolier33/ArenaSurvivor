using System;

namespace Railcar.Time
{
	public interface ITimer
	{
		IDisposable Mark(float delay, Action<float> callback);
	}
}