using System;
using Railcar;
using UniRx;
using UnityEngine;

namespace Bones.Gameplay.Utils
{
    public static class SnappingHelpers
    {
        public static int Snap(float source, float size)
        {
            return Mathf.RoundToInt(source / size);
        }

        public static Vector2Int Snap(Vector2 source, Vector2 size)
        {
            return new Vector2Int(Snap(source.x, size.x), Snap(source.y, size.y));
        }

        public static IObservable<Vector2Int> Snap(this IObservable<Vector2> source, Vector2 size)
        {
            return Observable.Select(source, x => Snap(x, size));
        }
        
        public static IObservable<Vector2Int> SnapDerivative(this IReadOnlyReactiveProperty<Vector2> source, Vector2 size)
        {
            return source.Select(x => Snap(x, size)).Derivative(Snap(source.Value, size));
        }
    }
}