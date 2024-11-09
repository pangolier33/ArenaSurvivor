using Cysharp.Threading.Tasks;
using UniRx;

namespace Bones.Gameplay.LoadOperations
{
    public class TestWaitOperation : ILoadOperation
    {
        public IReadOnlyReactiveProperty<float> Progress => _progress;
        public string Description => "Waiting 3 seconds...";
        
        private readonly ReactiveProperty<float> _progress = new();
        
        public async UniTask Execute()
        {
            _progress.Value = 0;
            await UniTask.Delay(1000);
            _progress.Value = 1 / 3f;
            await UniTask.Delay(1000);
            _progress.Value = 2 / 3f;
            await UniTask.Delay(1000);
            _progress.Value = 1f;
        }
    }
}