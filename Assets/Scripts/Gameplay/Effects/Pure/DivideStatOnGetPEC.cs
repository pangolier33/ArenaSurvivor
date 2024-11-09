using System;
using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Stats.Containers.Graphs;
using Bones.Gameplay.Stats.Utils;
using UnityEngine;

namespace Bones.Gameplay.Effects.Pure
{
	[Serializable]
	public sealed class DivideStatOnGetPEC : PureEffect
	{
		[SerializeField] private StatName _valueName;
		
		protected override void Invoke(ITrace trace)
		{
			var subjectStatName = trace.GetStatValue<StatName>(StatName.NameTag);
			var subjectStat = trace.GetStat(subjectStatName);
			var valueStat = trace.GetStat(_valueName);
			var modificationSubscription = subjectStat.DivideOnGet(valueStat);
			trace.ConnectToClosest(modificationSubscription);
		}
	}
}