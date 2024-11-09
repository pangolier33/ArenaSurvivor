using System;
using System.Threading;
using Railcar.Time;

namespace Bones.Gameplay.Utils
{
	public static class TimeExtensions
    {
        public static CancellationTokenSource MarkAndRepeat(this ITimer time, float delay, Action<float> callback)
        {
	        var cts = new CancellationTokenSource();
	        var token = cts.Token;
	        time.Mark(delay, x => time.MarkAndRepeat(x, delay, callback, token));
	        return cts;
        }

        public static CancellationTokenSource MarkAndRepeat(this ITimer time, Func<float> delayGetter, Action<float> callback)
        {
            var cts = new CancellationTokenSource();
	        time.Mark(delayGetter(), x => time.MarkAndRepeatWithGetter(x, delayGetter, callback, cts.Token));
            return cts;
        }

        private static void MarkAndRepeat(this ITimer time, float timestamp, float delay, Action<float> callback, CancellationToken token)
        {
	        if (token.IsCancellationRequested)
		        return;
	        callback(timestamp);
	        time.Mark(delay, x => time.MarkAndRepeat(x, delay, callback, token));
        }
        
        private static void MarkAndRepeatWithGetter(this ITimer time, float timestamp, Func<float> delayGetter, Action<float> callback, CancellationToken token)
        {
            if (token.IsCancellationRequested)
                return;
            callback(timestamp);
            var delay = delayGetter();
            time.Mark(delay, x => time.MarkAndRepeatWithGetter(x, delayGetter, callback, token));
        }
    }
}