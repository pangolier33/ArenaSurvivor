using System;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Bones.Gameplay.Waves.Spawning.Amounts
{
    [Serializable]
    public class RandomAmountResolver : IAmountResolver
    {
        [SerializeField] private Vector2Int _range;

        public RandomAmountResolver(){ }

        public RandomAmountResolver(int minInclusive, int maxInclusive)
        {
	        _range = new Vector2Int(minInclusive, maxInclusive);
        }
        
        public int GetAmount()
        {
            // TODO: random extension
            return Random.Range(_range.x, _range.y + 1);
        }
    }
    
    
    [Serializable]
    public class RandomFAmountResolver : IAmountFResolver
    {
	    [SerializeField] private float2 _range;

	    public RandomFAmountResolver(){ }

	    public RandomFAmountResolver(float minInclusive, float maxInclusive)
	    {
		    _range = new float2(minInclusive, maxInclusive);
	    }
        
	    public float GetAmount()
	    {
		    // TODO: random extension
		    return Random.Range(_range.x, _range.y + Mathf.Epsilon);
	    }
    }
}