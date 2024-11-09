using UnityEngine;
using Zenject;

namespace Bones.Gameplay.Factories
{
    public class PrefabFactory<T> : IFactory<T>, IValidatable where T : Object
    {
        private readonly DiContainer _container;
        private readonly T _prefab;
        private readonly Transform _parent;

        public PrefabFactory(DiContainer container, T prefab)
        {
            _container = container;
            _prefab = prefab;
            _parent = new GameObject($"{prefab.name}'s pool").transform;
        }

        public T Create()
        {
            return _container.InstantiatePrefabForComponent<T>(_prefab, _parent);
        }

        public void Validate()
        {
            _container.Instantiate<T>();
        }
    }
}