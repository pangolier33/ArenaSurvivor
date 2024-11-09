using System;
using System.Linq;
using UnityEngine;

namespace Railcar.UI
{
	[Serializable]
	internal sealed class CompositeBindingBuilder : BindingBuilder
	{
		[SerializeReference] private IBinding[] _bindings = default!;

		internal override IObserver<T> Build<T>() => _bindings.OfType<IObserver<T>>().First();
	}
}