using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Bones.Gameplay.LoadOperations
{
	public interface ILoadOperationExecutor
    {
        IObservable<ILoadOperation> ExecutingOperation { get; }
        UniTask Execute(IEnumerable<ILoadOperation> operations);
    }
}