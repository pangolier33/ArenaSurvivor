using System;
using Zenject;

namespace Bones.UI.Presenters
{
    public abstract class InjectedPresenter<T> : SinglePresenter, IInitializable
    {
        public void Initialize()
        {
            Subscribe(RetrieveModel());
        }
        
        protected abstract IObservable<T> RetrieveModel();
    }
}