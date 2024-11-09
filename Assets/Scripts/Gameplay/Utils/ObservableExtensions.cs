using System;
using UniRx;

namespace Bones.Gameplay.Inputs
{
	public static class ObservableExtensions
	{
		public static IReadOnlyReactiveProperty<T> Where<T>(this IReadOnlyReactiveProperty<T> property,  Func<T, bool> predicate)
		{
			var newProperty = new ReactiveProperty<T>();
			property.Subscribe(x =>
			{
				if (predicate.Invoke(x))
					newProperty.Value = x;
			});
			return newProperty;
		}
	}
}