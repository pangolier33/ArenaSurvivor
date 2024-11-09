using System;
using System.Collections.Generic;
using Bones.UI.Presenters.New;
using UniRx;
using UnityEngine;

namespace Bones.UI.Presenters
{
    public abstract class PrefabContainerPresenter : BaseBinder, IDisposable
    {
        [SerializeField] private PrefabPresenter _prefab;

        private readonly List<PrefabPresenter> _items = new();
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        protected void Clear()
        {
            foreach (var item in _items)
                Destroy(item.gameObject);
            _items.Clear();
        }
        
        protected PrefabPresenter Spawn<T>(IObservable<T> model)
        {
            var instance = _prefab.Spawn(model, transform);
            _items.Add(instance);
            return instance;
        }
        
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;
            
            foreach (var item in _items)
                item.Dispose();
            _items.Clear();
        }
        
        private void OnDestroy()
        {
            Dispose();
        }
    }
}