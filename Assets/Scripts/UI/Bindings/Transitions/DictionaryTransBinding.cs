using System;
using System.Collections.Generic;
using Bones.UI.Bindings.Transitions.Base;
using UnityEngine;

namespace Bones.UI.Bindings.Transitions
{
	[Serializable]
	public class DictionaryTransBinding<TKey, TValue> : TransBinding<TKey, TValue>
	{
		[SerializeReference] private Dictionary<TKey, TValue> _dictionary;
		
		protected override TValue Convert(TKey from) => _dictionary[from];
	}
}