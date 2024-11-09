using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Bones.Gameplay.Spells.Classes
{
	public interface ISpellModelWrapper
	{
		ISpellModel Wrapped { get; }
	}
	public abstract class SpellModelWrapper : ISpellModel, ISpellModelWrapper
	{
		protected SpellModelWrapper([NotNull] ISpellModel wrappedModel)
		{
			Wrapped = wrappedModel;
		}

		[NotNull] public ISpellModel Wrapped { get; }
		public string Name => Wrapped.Name;
		public string Description => Wrapped.Description;
		public Sprite Icon => Wrapped.Icon;
		public float Weight => Wrapped.Weight;
		public ISpellBranch OutBranch => Wrapped.OutBranch;
		
		public virtual IDisposable Apply()
		{
			return Wrapped.Apply();
		}


		public sealed override bool Equals(object other)
		{
			return Wrapped.Equals(other);
		}

		public sealed override int GetHashCode()
		{
			return Wrapped.GetHashCode();
		}

		public override string ToString()
		{
			return Wrapped.ToString();
		}

		protected bool Equals(SpellModelWrapper other)
		{
			return Equals(Wrapped, other.Wrapped);
		}
	}
}