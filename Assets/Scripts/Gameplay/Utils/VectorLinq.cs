using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Bones.Gameplay.Utils
{
    public static class VectorLinq
    {
        public static Vector3 Sum(this IEnumerable<Vector3> source)
        {
            return source.Aggregate(Vector3.zero, (current, v3) => current + v3);
        }
        public static Vector2 Sum(this IEnumerable<Vector2> source)
        {
            return source.Aggregate(Vector2.zero, (current, v2) => current + v2);
        }
    }
}