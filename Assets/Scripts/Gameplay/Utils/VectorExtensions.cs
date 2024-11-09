using UnityEngine;

namespace Bones.Gameplay.Utils
{
    public static class VectorExtensions
    {
        public static bool Approximately(this Vector2 first, Vector2 second)
        {
            return Mathf.Approximately(first.x, second.x) && Mathf.Approximately(first.y, second.y);
        }
        
        public static Vector3 PickRandomCirclePoint(Vector3 from, Vector2 range, bool topOnly = true)
        {
            return PickRandomCirclePoint(from, range.Random(), topOnly);
        }

        public static Vector3 PickRandomCirclePoint(Vector3 from, float range, bool topOnly = true)
        {
            Vector3 pos = UnityEngine.Random.insideUnitCircle;
            pos = pos.normalized * range;
            if (topOnly) pos.y = Mathf.Abs(pos.y);
            return from + pos;
        }

        public static float SqrDistanceTo(this Vector3 from, Vector3 to)
        {
            return (to - from).sqrMagnitude;
        }

        public static float DistanceTo(this Vector3 from, Vector3 to)
        {
            return (to - from).magnitude;
        }

        public static float Lerp(this Vector2 minMax, float value)
        {
            return Mathf.Lerp(minMax.x, minMax.y, value);
        }

        public static bool Contains(this Vector2 minMax, float value)
        {
            return value > minMax.x && value < minMax.y;
        }

        public static bool IsPositionInRadius(Vector3 center, float radius, Vector3 point)
        {
            bool x = point.x < center.x + radius && point.x > center.x - radius;
            bool y = point.y < center.y + radius && point.y > center.y - radius;
            return x && y;
        }

        public static float Random(this Vector2 minMax)
        {
            return UnityEngine.Random.Range(minMax.x, minMax.y);
        }

        public static Vector2 RandomRadius(this Vector2 position, float radius)
        {
            return position + UnityEngine.Random.insideUnitCircle * radius;
        }

        public static Vector2 RandomSized(this Vector2 position, Vector2 size)
        {
            return position + Vector2.Scale(size,
                new Vector2(UnityEngine.Random.value - .5f, UnityEngine.Random.value - .5f));
        }

        public static int Random(this Vector2Int minMax)
        {
            return UnityEngine.Random.Range(minMax.x, minMax.y);
        }

        public static float Random(this Vector4 minMax, int XYOrZW = 0)
        {
            if (XYOrZW == 0) return UnityEngine.Random.Range(minMax.x, minMax.y);

            return UnityEngine.Random.Range(minMax.z, minMax.w);
        }
    }
}