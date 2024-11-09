using System;
using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Effects.Transitive.Depth;
using Bones.Gameplay.Stats.Containers;
using Bones.Gameplay.Stats.Containers.Graphs;
using Bones.Gameplay.Stats.Units;
using Bones.Gameplay.Stats.Utils;
using UniRx;
using UnityEngine;

namespace Bones.Gameplay.Effects.Transitive.Width
{
	[Serializable]
	public class InvokeOnAddWTEC : TransitiveEffect
	{
		[SerializeField] private StatName _statName;
		protected override void Invoke(IMutableTrace trace, IEffect next)
		{
			var stat = trace.GetStat(_statName);
			var observable = stat switch
			{
				IAddStat<Value> valueStat => valueStat.ObserveOnAdd().Subscribe(x => next.Invoke(trace.Add(CreateStatMap(x)))),
				IAddStat<Points> pointsStat => pointsStat.ObserveOnAdd().Subscribe(x => next.Invoke(trace.Add(CreateStatMap(x)))),
				_ => Disposable.Empty
			};
			trace.ConnectToClosest(observable);

			IStatMap CreateStatMap<T>(T value) => new UnitStatMap(StatName.Delta, new ReadOnlyStat<T>(value));
		}
	}
}