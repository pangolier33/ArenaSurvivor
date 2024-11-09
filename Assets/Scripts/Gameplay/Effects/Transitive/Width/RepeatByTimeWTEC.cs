using System;
using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Stats.Containers.Graphs;
using Bones.Gameplay.Stats.Units;
using Bones.Gameplay.Utils;
using Railcar.Time;
using UnityEngine;
using Zenject;

namespace Bones.Gameplay.Effects.Transitive.Width
{
	[Serializable]
	public sealed class RepeatByTimeWTEC : TransitiveEffect
	{
		[SerializeField] private StatName _name;
        
		private ITimer _timer;
        
		protected override void Invoke(IMutableTrace trace, IEffect next)
		{
			var subscription = _timer.MarkAndRepeat(() => trace.GetStatValue<Value>(_name), _ => next.Invoke(trace));
			trace.ConnectToClosest(subscription);
		}
		
		[Inject]
		private void Inject([Inject(Id = TimeID.Fixed)] ITimer timer)
		{
			_timer = timer;
		}
	}
}