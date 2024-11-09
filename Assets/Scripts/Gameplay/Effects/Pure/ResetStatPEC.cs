using System;
using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Stats.Containers;
using Bones.Gameplay.Stats.Containers.Graphs;
using Bones.Gameplay.Stats.Units;
using UnityEngine;

namespace Bones.Gameplay.Effects.Pure
{
	[Serializable]
	public class ResetStatPEC : PureEffect
	{
		[SerializeField] private StatName _name;
		
		protected override void Invoke(ITrace trace)
		{
			switch (trace.GetStat(_name))
			{
				case ISetStat<Value> setValueStat:
				{
					setValueStat.Set(Value.Zero);
					break;
				}
			}
		}
	}
}