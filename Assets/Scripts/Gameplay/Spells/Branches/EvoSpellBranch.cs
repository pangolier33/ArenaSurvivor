using System;
using System.Collections.Generic;
using System.Linq;
using Bones.Gameplay.Events.Callbacks.Templates.Base;
using UnityEngine;
using Zenject;

namespace Bones.Gameplay.Spells.Classes
{
	[Serializable]
	public sealed class SingleSpellBranch : ISpellBranch, IInjectable
	{
		[SerializeField] private BaseSpellBranch _spellBranch;
		public bool IsActive => _spellBranch.IsActive;
		public bool IsAvailable => _spellBranch.IsAvailable;
		public ISpellModel Model => _spellBranch.Model;

		public override bool Equals(object obj)
		{
			return _spellBranch != null && _spellBranch.Equals(obj);
		}

		public override int GetHashCode()
		{
			return _spellBranch == null ? 0 : _spellBranch.GetHashCode();
		}

		[Inject]
		private void Inject(DiContainer container)
		{
			container.Inject(_spellBranch);
		}
	}

	[Serializable]
	public class EvoSpellBranch : ISpellBranch, IInjectable
	{
		[SerializeField] private List<BaseSpellBranch> _branches = default!;

		public EvoSpellBranch() => Model = new SpellModelWrapper(this);

		public bool IsActive => OutBranch.IsActive;
		public bool IsAvailable => OutBranch.IsAvailable;
		public ISpellModel Model { get; }

		private ISpellBranch OutBranch => _branches.FirstOrDefault(x => x.IsAvailable) ?? _branches.Last();

		[Inject]
		private void Inject(DiContainer container)
		{
			foreach (var branch in _branches)
				container.Inject(branch);
		}
		
		private class SpellModelWrapper : ISpellModel, ISpellModelWrapper
		{
			private readonly EvoSpellBranch _origin;
			private IDisposable _cachedDescription;

			public SpellModelWrapper(EvoSpellBranch origin)
			{
				_origin = origin;
			}

			public string Name => _origin.OutBranch.Model.Name;
			public string Description => _origin.OutBranch.Model.Description;
			public Sprite Icon => _origin.OutBranch.Model.Icon;
			public float Weight => _origin.OutBranch.Model.Weight;
			public ISpellBranch OutBranch => _origin.OutBranch;
			public ISpellModel Wrapped => _origin.OutBranch.Model;

			public IDisposable Apply()
			{
				var newSubscription = _origin.OutBranch.Model.Apply();
				if (_cachedDescription != null && !_cachedDescription.Equals(newSubscription))
					_cachedDescription.Dispose();
				return _cachedDescription = newSubscription;
			}
		}
	}
}