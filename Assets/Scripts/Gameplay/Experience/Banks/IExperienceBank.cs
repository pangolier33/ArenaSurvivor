using System;
using UniRx;

namespace Bones.Gameplay.Experience
{
    public interface IExperienceBank
    {
        void Obtain(float amount);
        void ObtainPercent(float percent);
        IReadOnlyReactiveProperty<int> Level { get; }
        IObservable<IStarModel> Requested { get; } 
        IObservable<IStarModel> Released { get; }
    }
}