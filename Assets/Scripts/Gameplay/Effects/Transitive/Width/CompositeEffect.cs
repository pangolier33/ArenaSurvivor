using System;
using System.Collections.Generic;
using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Events.Callbacks.Templates.Base;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Bones.Gameplay.Effects.Transitive.Width
{
	[Serializable]
	public class CompositeEffect : IEffect, IInjectable
	{
		[SerializeReference, ListDrawerSettings(ListElementLabelName = "@$property.BaseValueEntry.WeakSmartValue.GetType()?.Name", Expanded = true), HideReferenceObjectPicker]
		private IEffect[] _effects = Array.Empty<IEffect>();

		public void Invoke(IMutableTrace trace)
		{
			var exceptions = new Lazy<List<Exception>>();
			trace = trace.Add("List");
			foreach (var effect in _effects)
			{
				try
				{
					effect.Invoke(trace);
				}
				catch (Exception exception)
				{
					exceptions.Value.Add(exception);
				}
			}

			if (exceptions.IsValueCreated)
				throw new AggregateException(exceptions.Value);
		}

		[Inject]
		private void Inject(DiContainer container)
		{
			foreach (var effect in _effects)
			{
				if (effect is IInjectable)
					container.Inject(effect);
			}
		}
	}
}