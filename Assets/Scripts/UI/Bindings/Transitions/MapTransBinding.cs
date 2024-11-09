using System;
using Bones.UI.Bindings.Transitions.Base;
using UnityEngine;

namespace Bones.UI.Bindings.Transitions
{
	[Serializable]
	public class MapTransBinding<T> : TransBinding<int, T>
	{
		[SerializeField] private T[] _values;
		protected override T Convert(int from) => _values[from];
	}
}