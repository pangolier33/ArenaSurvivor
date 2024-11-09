using System;
using UnityEngine;
using Zenject;

namespace Bones.Gameplay.Factories.Pools.Mono
{
    public abstract class MonoPoolBridge<TEntity> : MonoBehaviour, IDisposable
        where TEntity : MonoPoolBridge<TEntity>
    {
        private Pool _pool;

        protected abstract void OnSpawned();
        protected abstract void OnDespawned();
        
        public virtual void Dispose()
        {
            _pool?.Despawn((TEntity)this);
        }
        
        private void OnDestroy()
        {
            //if (_pool == null)
                //throw new InvalidOperationException("Pooled entity cannot be destroyed directly. Use Dispose() instead");
        }

        public sealed class Pool : MonoPool<TEntity>
        {
            public Pool(IFactory<TEntity> factory) : base(factory) { }

            protected override void OnSpawned(TEntity entity)
            {
                base.OnSpawned(entity);
                entity._pool = this;
                entity.OnSpawned();
            }

            protected override void OnDespawned(TEntity entity)
            {
				if (entity._pool == null)
					return;
                entity._pool = null;

				base.OnDespawned(entity);
                entity.OnDespawned();
            }
        }
    }
    
    public abstract class MonoPoolBridge<TParam, TEntity> : MonoBehaviour, IDisposable
        where TEntity : MonoPoolBridge<TParam, TEntity>
        where TParam : notnull
    {
        private Pool _pool;

        public bool IsSpawned => _pool != null;
        protected TParam Settings { get; private set; }

        protected abstract void OnSpawned();
        protected abstract void OnDespawned();
        
        public void Dispose()
        {
            if (!IsSpawned)
                return;
            _pool.Despawn((TEntity)this);
        }
        
        private void OnDestroy()
        {
            //if (_pool == null)
                //throw new InvalidOperationException($"[{GetType().Name}] OnDestroy | Pooled entity '{name}' cannot be destroyed directly. Use Dispose() instead");
        }

        public class Pool : MonoPool<TParam, TEntity>
        {
            public Pool(IFactory<TEntity> factory) : base(factory) { }

            protected override void OnSpawned(TEntity entity, TParam param)
            {
                entity._pool = this;
                entity.Settings = param;
                
                entity.OnSpawned();
                base.OnSpawned(entity, param);
            }

            protected override void OnDespawned(TEntity entity)
            {
				if (entity._pool == null)
					return;
                entity._pool = null;


                base.OnDespawned(entity);
                entity.OnDespawned();
                
                entity.Settings = default;
            }
        }
    }
}