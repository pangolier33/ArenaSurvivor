using System;
using System.Threading;
using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Stats.Containers.Graphs;
using Bones.Gameplay.Stats.Units;
using Railcar.Time;
using UnityEngine;
using Zenject;

namespace Bones.Gameplay.Effects.Transitive.Width
{
	[Serializable]
	public sealed class WaitDTEC : TransitiveEffect
	{
		[SerializeField] private StatName _name;

		private ITimer _timer;
		
		protected override void Invoke(IMutableTrace trace, IEffect next)
		{
			var cts = new CancellationTokenSource();
			var delay = (float)trace.GetStatValue<Value>(_name);
			_timer.Mark(delay, _ =>
			                   {
				                   if (cts.IsCancellationRequested)
					                   return;
				                   next.Invoke(trace);
			                   });
			trace.ConnectToClosest(cts);
		}

		[Inject]
		private void Inject([Inject(Id = TimeID.Fixed)] ITimer time)
		{
			_timer = time;
		}
	}
}