using System;

namespace Bones.Gameplay.Experience
{
    public interface IStarModel
    {
        IObservable<float> Completeness { get; }
    }
}