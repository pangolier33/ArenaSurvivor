using Bones.Gameplay.Factories.Pools.Base;
using UnityEngine;
using Zenject;

namespace Bones.Gameplay.Factories.Pools.Mono
{
    public class GameObjectPool : BasePool<GameObject>
    {
        private readonly IFactory<GameObject> _factory;

        public GameObjectPool(IFactory<GameObject> factory)
        {
            _factory = factory;
        }

        protected override GameObject Instantiate()
        {
            return _factory.Create();
        }

        protected override void OnSpawned(GameObject entity)
        {
            entity.SetActive(true);
        }

        protected override void OnDespawned(GameObject entity)
        {
            entity.SetActive(false);
        }
    }
}