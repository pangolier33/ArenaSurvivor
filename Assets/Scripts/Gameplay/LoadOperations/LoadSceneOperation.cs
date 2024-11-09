using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine.AddressableAssets;

namespace Bones.Gameplay.LoadOperations
{
    public class LoadSceneOperation : ILoadOperation
    {
        private readonly AssetReference _scene;
        private readonly ReactiveProperty<float> _progress = new();

        public IReadOnlyReactiveProperty<float> Progress => _progress;
        public string Description => "Loading scene...";

        public LoadSceneOperation(AssetReference scene)
        {
            _scene = scene;
        }
        
        public async UniTask Execute()
        {
            var operation = Addressables.LoadSceneAsync(_scene);
            while (!operation.IsDone)
            {
                _progress.Value = operation.PercentComplete;
                await UniTask.Delay(1);
            }
        }
    }
}