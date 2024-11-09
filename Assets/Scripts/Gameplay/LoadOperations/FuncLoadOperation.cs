using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UniRx;

namespace Bones.Gameplay.LoadOperations
{
    public class FuncLoadOperation : ILoadOperation
    {
        private readonly ReactiveProperty<float> _progress = new();
        private readonly Func<ReactiveProperty<float>, Task> _func;
        public string Description { get; }

        public IReadOnlyReactiveProperty<float> Progress => _progress;

        public FuncLoadOperation(Func<ReactiveProperty<float>, Task> func, string description)
        {
            _func = func;
            Description = description;
        }

        public async UniTask Execute()
        {
            await _func.Invoke(_progress);
        }
    }
}