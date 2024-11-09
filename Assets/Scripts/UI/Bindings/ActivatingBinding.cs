using System;
using Bones.UI.Bindings.Base;
using UnityEngine;

namespace UI.Bindings
{
	[Serializable]
	public class ActivatingBinding : BaseBinding<int>
	{
		[SerializeField] private GameObject[] _handles;

		public override void OnNext(int value)
		{
			for (var i = 0; i < _handles.Length; i++)
				_handles[i].SetActive(i < value);
		}
	}
}