using System;
using Bones.UI.Presenters;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Bones.UI.Bindings.Base
{
	public class CompositeTransBinding<T> : BaseBinding<T>
	{
		[SerializeReference]
		[InlineProperty]
		[HideLabel]
		private IBinding[] _bindings;
		
		public override void OnNext(T value)
		{
			var exception = new ExceptionsHandler();
			foreach (var binding in _bindings)
			{
				if (binding is not IObserver<T> observer)
				{
					exception.Add(new InvalidCastException($"{binding} is not observing {typeof(T)}"));
					continue;
				}
				observer.OnNext(value);
			}
		}
	}
}