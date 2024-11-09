using System;
using UniRx;
using UnityEngine;

namespace Railcar
{
    public static class ObservableExtensions
    {
        public static IObservable<int> AsCounter<T>(this IObservable<T> source)
        {
            var property = new ReactiveProperty<int>();
            source.Subscribe(_ => property.Value++);
            return property;
        }

        public static IObservable<T> Derivative<T>(this IObservable<T> source, T initialValue, Func<T, T, T> subtractingFunction)
        {
            var property = new ReactiveProperty<T>(initialValue);
            return source.Select(newValue =>
            {
                var diff = subtractingFunction.Invoke(newValue, property.Value);
                property.Value = newValue;
                return diff;
            });
        }
        
        public static IObservable<Vector2Int> Derivative(this IObservable<Vector2Int> source, Vector2Int initialValue)
        {
            return source.Derivative(initialValue, (oldValue, newValue) => newValue - oldValue);
        }
        
        public static IObservable<T> Derivative<T>(this IReadOnlyReactiveProperty<T> source, Func<T, T, T> subtractingFunction)
        {
            return source.Derivative(source.Value, subtractingFunction);
        }
        
        public static IObservable<Vector2Int> Derivative(this IReadOnlyReactiveProperty<Vector2Int> source)
        {
            return source.Derivative((oldValue, newValue) => newValue - oldValue);
        }
        
        public static IObservable<Vector2> Derivative(this IReadOnlyReactiveProperty<Vector2> source)
        {
            return source.Derivative((oldValue, newValue) => newValue - oldValue);
        }
    }
}