using Bones.Gameplay.Factories.Pools.Base;
using UnityEngine;
using Zenject;

namespace Bones.Gameplay.Factories.Pools.Mono
{
    public class ComponentPool<T> : BasePool<T>
        where T : Component
    {
        private readonly IFactory<GameObject> _factory;

        public ComponentPool(IFactory<GameObject> factory)
        {
            _factory = factory;
        }

        protected override T Instantiate()
        {
            var gameObject = _factory.Create();
            var component = gameObject.AddComponent<T>();
            return component;
        }

        protected override void OnDespawned(T entity)
        {
            entity.gameObject.SetActive(false);
        }

        protected override void OnSpawned(T entity)
        {
            entity.gameObject.SetActive(true);
        }
    }
}