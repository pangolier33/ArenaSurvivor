using System;
using System.Collections;
using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Stats.Containers.Graphs;
using Bones.Gameplay.Stats.Units;
using UnityEngine;

namespace Bones.Gameplay.Effects.Transitive.Width
{
	[Serializable]
	public sealed class RetrieveWTEC : TransitiveEffect
	{
		[SerializeField] private StatName _countName;
        
		protected override void Invoke(IMutableTrace trace, IEffect next)
		{
			var count = 0;
			var limit = (int)trace.GetStatValue<Amount>(_countName);
			var enumerator = trace.Get<IEnumerator>();

			while (enumerator.MoveNext() && count++ < limit)
				next.Invoke(trace.Add(enumerator.Current));
		}
	}
}