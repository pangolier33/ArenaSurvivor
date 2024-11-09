using System;
using Bones.Gameplay.Events.Callbacks.Templates.Base;
using Bones.Gameplay.LoadOperations;
using UniRx;
using UnityEngine;
using Zenject;

namespace Bones.UI.Presenters
{
    public class LoadScreenPresenter : MonoBehaviour, IInitializable, IDisposable, IInjectable
    {
        [SerializeField] private LoadOperationPresenter _operationPresenter;
        private ILoadOperationExecutor _loadOperationExecutor;
        private IDisposable _operationSubscription;

        [Inject]
        private void Inject(ILoadOperationExecutor loadOperationExecutor)
        {
            _loadOperationExecutor = loadOperationExecutor;
        }

        public void Initialize()
        {
            _operationSubscription = _loadOperationExecutor.ExecutingOperation.Subscribe(OnNextOperation);
            gameObject.SetActive(false);
        }

        public void Dispose()
        {
            _operationSubscription?.Dispose();
        }

        private void OnNextOperation(ILoadOperation loadOperation)
        {
            gameObject.SetActive(true);
            _operationPresenter.OnNext(loadOperation);
        }
    }
}