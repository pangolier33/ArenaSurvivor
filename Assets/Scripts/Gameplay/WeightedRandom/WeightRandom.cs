using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

namespace Bones.Gameplay.WeightedRandom
{
	public static class WeightRandom
    {
        public static IEnumerable<int> GetIndexes(IEnumerable<float> weights, int count)
        {
            var weightsList = weights.OrderBy(_ => Random.value).ToList();
            var selectedSet = new HashSet<int>(count);
            var totalWeight = weightsList.Sum();
            
            while (weightsList.Count - selectedSet.Count > 0 && count - selectedSet.Count > 0)
            {
                var rand = Random.Range(0, totalWeight);
                for (int i = 0; i < weightsList.Count; i++)
                {
                    if (selectedSet.Contains(i))
                    {
                        continue;
                    }

                    if (weightsList[i] >= rand)
                    {
                        totalWeight -= weightsList[i];
                        selectedSet.Add(i);
                        yield return i;
                        break;
                    }

                    rand -= weightsList[i];
                }
            }
        }

        public static int GetIndex(IEnumerable<float> weights)
        {
            var indexes = GetIndexes(weights, 1).ToList();
            if (indexes.Count > 0) return indexes[0];
            return -1;
        }
    }
}