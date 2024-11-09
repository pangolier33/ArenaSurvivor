using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;

namespace Bones.Gameplay.LoadOperations
{
	public class LoadOperationExecutor : ILoadOperationExecutor
    {
        private readonly Subject<ILoadOperation> _executingOperation = new();
        
        public IObservable<ILoadOperation> ExecutingOperation => _executingOperation;

        public async UniTask Execute(IEnumerable<ILoadOperation> operations)
        {
            foreach (var loadOperation in operations)
            {
                _executingOperation.OnNext(loadOperation);
                await loadOperation.Execute();
            }
        }
    }
}