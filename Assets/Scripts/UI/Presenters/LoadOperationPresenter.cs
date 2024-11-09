using Bones.Gameplay.LoadOperations;
using Bones.UI.Bindings.Base;
using UniRx;
using UnityEngine;

namespace Bones.UI.Presenters
{
    public class LoadOperationPresenter : BasePresenter
    {
        [SerializeReference] private IBinding _progressBinding;
        [SerializeReference] private IBinding _descriptionBinding;
        
        public void OnNext(ILoadOperation operation)
        {
            OnNext(operation.Description, _descriptionBinding);
            OnNext(0f, _progressBinding);
            Subscribe(operation.Progress, _progressBinding);
        }
    }
}