using Cysharp.Threading.Tasks;
using UniRx;

namespace Bones.Gameplay.LoadOperations
{
    public interface ILoadOperation
    {
        IReadOnlyReactiveProperty<float> Progress { get; }
        string Description { get; }
        UniTask Execute();
    }
}