using System;
using Bones.Gameplay.Effects.Containers;
using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Events.Callbacks.Templates.Base;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Bones.Gameplay.Spells.Classes
{
	public abstract class BaseSpellBranch : ScriptableObject, ISpellModel, ISpellBranch, IInjectable
	{
		private IMutableTrace _trace;

		// TODO: add validation

		[field: SerializeField, TabGroup("Meta", Paddingless = true), PropertyOrder(1)] public string Name { get; private set; }
		[field: SerializeField, TabGroup("Meta", Paddingless = true), PropertyOrder(1)] private ScriptableEffectContainer Effect { get; set; }
		[field: SerializeField, TabGroup("Meta", Paddingless = true), PropertyOrder(1)] public Sprite Icon { get; private set; }

		public ISpellModel Model => this;
		public ISpellBranch OutBranch => this;

		public abstract string Description { get; }
		public abstract bool IsActive { get; }
		public abstract bool IsAvailable { get; }
		public abstract float Weight { get; }

		IDisposable ISpellModel.Apply()
		{
			if (!IsAvailable)
				throw new InvalidOperationException("Cannot apply unavailable spell");
			return ApplyWhenAvailable();
		}

		public override bool Equals(object other)
		{
			if (other is SpellModelWrapper wrapper)
				return Equals(wrapper.Wrapped);
			return base.Equals(other);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override string ToString() => $"Spell {Name}{(IsActive ? ", active" : "")}{(IsAvailable ? ", available" : "")}";

		protected abstract void OnReset();
		protected virtual IDisposable ApplyWhenAvailable() => Effect.Invoke(_trace);
		protected virtual IMutableTrace AppendTraceOnInject(DiContainer container, IMutableTrace trace) => trace;

		[Inject]
		private void Inject(DiContainer container, IMutableTrace trace)
		{
			OnReset();
			container.Inject(Effect);
			_trace = AppendTraceOnInject(container, trace);
		}
	}
}