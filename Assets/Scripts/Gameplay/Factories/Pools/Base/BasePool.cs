using System.Collections.Generic;

namespace Bones.Gameplay.Factories.Pools.Base
{
    public abstract class BasePool<T> : IPool<T>
    {
        private readonly Stack<T> _disabledEntities = new();
        
        public ICollection<T> Entities { get; } = new Collection<T>();

        public T Create()
        {
            if (!_disabledEntities.TryPop(out var entity))
            {
                entity = Instantiate();
            }
            
            OnSpawned(entity);
            Entities.Add(entity);
            return entity;
        }

        public void Despawn(T entity)
        {
            _disabledEntities.Push(entity);
            OnDespawned(entity);
            Entities.Remove(entity);
        }

        protected abstract T Instantiate();
        protected abstract void OnSpawned(T entity);
        protected abstract void OnDespawned(T entity);
    }

    public abstract class BasePool<TParam, TEntity> : IPool<TParam, TEntity>
    {
        private readonly Stack<TEntity> _disabledEntities = new();

        public ICollection<TEntity> Entities { get; } = new Collection<TEntity>();

        public TEntity Create(TParam param)
        {
            if (!_disabledEntities.TryPop(out var entity))
            {
                entity = Instantiate();
            }

            OnSpawned(entity, param);
            Entities.Add(entity);
            return entity;
        }

        public void Despawn(TEntity entity)
        {
            _disabledEntities.Push(entity);
            OnDespawned(entity);
            Entities.Remove(entity);
        }

        protected abstract TEntity Instantiate();
        protected abstract void OnSpawned(TEntity entity, TParam param);
        protected abstract void OnDespawned(TEntity entity);
    }
}