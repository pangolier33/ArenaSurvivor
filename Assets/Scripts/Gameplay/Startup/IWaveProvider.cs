using System;
using Bones.Gameplay.Waves;

namespace Bones.Gameplay.Startup
{
    public interface IWaveProvider
    {
        IObservable<WaveConfig> Wave { get; }
        IObservable<int> Index { get; }
        IObservable<float> Completeness { get; }
        float GeneralTime { get; }
		float TimeSpent { get; }
    }
}