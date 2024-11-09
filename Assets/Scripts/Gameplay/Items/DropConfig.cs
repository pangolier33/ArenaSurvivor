using System;
using System.Linq;
using Bones.Gameplay.Factories.Pools.Base;
using Bones.Gameplay.WeightedRandom;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;
using Random = UnityEngine.Random;

namespace Bones.Gameplay.Items
{
	[CreateAssetMenu]
    public class DropConfig : ScriptableObject
    {
        [FormerlySerializedAs("SelectLogic")]
        [SerializeField]
        private SelectLogic _selectLogic;
        
        [FormerlySerializedAs("Items")]
        [SerializeField]
        [TableList]
        private ItemDropData[] _items;
        
        [SerializeField]
        private float _spawningRadius;

        private IFactory<Item, IPool<Item>> _poolFactory;
        
        public void SetPoolFactory(IFactory<Item, IPool<Item>> poolFactory)
        {
            _poolFactory = poolFactory;
        }

        public void Spawn(Vector2 position)
        {
            switch (_selectLogic)
            {
                case SelectLogic.AllByChance:
                {
                    for (int i = 0; i < _items.Length; i++)
                    {
                        if (Random.value * 100 <= _items[i].Chance)
                        {
                            Spawn(position, _items[i]);
                        }
                    }

                    break;
                }

                case SelectLogic.OneByWeight:
                {
                    int selected = WeightRandom.GetIndex(_items.Select(x => x.Chance));
                    if (selected >= 0)
                    {
                        Spawn(position, _items[selected]);
                    }

                    break;
                }

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected void Spawn(Vector2 position, ItemDropData data)
        {
            var countRange = data.Count;
            var count = Random.Range(countRange.x, countRange.y);

            for (var i = 0; i < count; i++)
            {
                var pool = _poolFactory.Create(data.ItemPrefab);
                var spawned = pool.Create();
                var offsetPosition = Random.insideUnitCircle * _spawningRadius;
                var finalPosition = (Vector3)(position + offsetPosition);
                spawned.transform.position = finalPosition;
            }
        }
    }
}