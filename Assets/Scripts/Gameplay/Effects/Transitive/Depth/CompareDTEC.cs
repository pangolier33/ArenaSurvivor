using System;
using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Events.Callbacks.Templates.Base;
using Bones.Gameplay.Stats.Containers.Graphs;
using UnityEngine;
using Zenject;

namespace Bones.Gameplay.Effects.Transitive.Depth
{
	public abstract class CompareDTEC<T> : IEffect, IInjectable
		where T : IComparable<T>
	{
		[SerializeField] private StatName _leftName;
		[SerializeField] private StatName _rightName;
		[SerializeReference] private IEffect _onGreaterEffect;
		[SerializeReference] private IEffect _onEqualEffect;
		[SerializeReference] private IEffect _onLessEffect;
		
		public void Invoke(IMutableTrace trace)
		{
			switch (trace.GetStatValue<T>(_leftName).CompareTo(trace.GetStatValue<T>(_rightName)))
			{
				case 0:
					_onEqualEffect.Invoke(trace);
					return;
				case > 0:
					_onGreaterEffect.Invoke(trace);
					return;
				case < 0:
					_onLessEffect.Invoke(trace);
					return;
			}
		}

		[Inject]
		private void Inject(DiContainer container)
		{
			container.Inject(_onGreaterEffect);
			container.Inject(_onEqualEffect);
			container.Inject(_onLessEffect);
		}
	}
}