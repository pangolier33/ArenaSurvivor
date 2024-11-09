using System;
using UnityEngine;

namespace Bones.UI.Presenters
{
    public class PrefabPresenter : SinglePresenter
    {
        private bool _isInstance;
        
        public PrefabPresenter Spawn<T>(IObservable<T> model, Transform parent)
        {
            if (_isInstance)
                throw new InvalidOperationException("Can't spawn prefab's instance");
            
            var instance = Instantiate(this, parent);
            instance._isInstance = true;
            
            instance.Subscribe(model);
            return instance;
        }
    }
}