using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Bones.Gameplay.Utils
{
	public static class LinqExtensions
	{
		public static IEnumerable<T> Log<T>(this IEnumerable<T> enumerable)
		{
			// ReSharper disable once PossibleMultipleEnumeration
			foreach (var item in enumerable)
				Debug.Log(item);
			// ReSharper disable once PossibleMultipleEnumeration
			return enumerable;
		}
		
		public static IEnumerable<T> WeightedMix<T>(this IEnumerable<T> source, int countLimit, [NotNull] Func<T, float> weightGetter)
		{
			switch (countLimit)
			{
				case < 0:
					throw new ArgumentOutOfRangeException(nameof(countLimit), countLimit, "Count must be positive");
				case 0:
					return Enumerable.Empty<T>();
				default:
				{
					var items = source.OrderBy(_ => Random.value).ToList();
					return items.Count <= countLimit ? items : WeightedMix(items, countLimit, weightGetter);
				}
			}
		}

		private static IEnumerable<T> WeightedMix<T>(IList<T> source, int countLimit, Func<T, float> weightGetter)
		{
			var totalWeight = source.Sum(weightGetter);
			while (countLimit-- > 0)
			{
				// TODO: use dice
				var pickedWeight = Random.Range(0, totalWeight);
				for (var i = 0; i < source.Count; i++)
				{
					var currentWeight = weightGetter(source[i]); 
					pickedWeight -= currentWeight;
					totalWeight -= currentWeight;
                    
					if (pickedWeight >= 0)
						continue;
                    
					yield return source[i];
					source.RemoveAt(i);
					break;
				}
			}
		}
	}
}