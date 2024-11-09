using System;
using System.Collections.Generic;
using Bones.UI.Bindings.Base;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UI.Bindings
{
	[Serializable]
	public sealed class PrefabSpawningBinding : BaseBinding<int>
	{
		[SerializeField] private GameObject _prefab;
		[SerializeField] private Transform _parent;

		private readonly Stack<GameObject> _instances = new();

		public override void OnNext(int value)
		{
			if (value < 0)
				throw new ArgumentOutOfRangeException(nameof(value), value, "Items count cannot be negative");
			
			while (_instances.Count > value)
				Object.Destroy(_instances.Pop());
			while (_instances.Count < value)
				_instances.Push(Object.Instantiate(_prefab, _parent));
		}
	}
}