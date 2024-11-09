using System;
using System.Text.RegularExpressions;
using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Events.Callbacks.Templates.Base;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Bones.Gameplay.Effects.Transitive
{
	public abstract class TransitiveEffect : IEffect, IInjectable
	{
		//[Title("Next")]
		
		[Title("@\"Next:  \" + $property.BaseValueEntry.WeakSmartValue?.GetType().Name")]
		[HideReferenceObjectPicker]

		[SerializeReference]
		[InlineProperty]
		[HideLabel]
		[PropertyOrder(float.MaxValue)]
		[Space(16)]
		private IEffect _embeddedEffect;

		private static readonly Regex s_typeNameExpr = new(@"(?<=[A-Z])(?=[A-Z][a-z])|(?<=[^A-Z])(?=[A-Z])|(?<=[A-Za-z])(?=[^A-Za-z])");

		public void Invoke(IMutableTrace trace)
		{
            #if UNITY_EDITOR
			trace = trace.Add(this);
            #endif
			Invoke(trace, _embeddedEffect);
		}

		protected abstract void Invoke(IMutableTrace trace, IEffect next);

		[Inject]
		private void Inject(DiContainer container)
		{
			switch (_embeddedEffect)
			{
				case null:
					throw new NullReferenceException($"Effect is not chosen for transitive effect {GetType().Name}");
				case IInjectable:
					container.Inject(_embeddedEffect);
					break;
			}
		}

		public override string ToString()
		{
			return s_typeNameExpr.Replace(GetType().Name, " ");
		}
	}
}