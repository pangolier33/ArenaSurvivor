using System.Collections.Generic;
using Bones.Gameplay.Events.Callbacks.Templates.Base;
using Bones.Gameplay.Experience;
using Bones.Gameplay.Meta.CharacterClases;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Bones.Gameplay.Spells.Classes
{
	[CreateAssetMenu]
	public sealed class ClassContainer : BaseSpellContainer, IInjectable, IClassContainer
	{
		private const int LevelRequiredForUlt = 10;
		private IExperienceBank _experienceBank;

		[SerializeReference]
		[InlineEditor]
		private ISpellBranch _baseBranch;

		[SerializeReference]
		[InlineEditor]
		[Indent(0)]
		private ISpellBranch[] _commonBranches;

		[SerializeReference]
		[InlineEditor]
		private ISpellBranch _ultBranch;

		[field: SerializeField]
		[field: PropertyOrder(-1)]
		public ClassName Name { get; private set; }
		[field: SerializeField]
		[field: PropertyOrder(-1)]
		public Sprite Icon { get; private set; }

		public override IEnumerable<ISpellModel> GetModels()
		{
			yield return RetrieveModel(_baseBranch, SpellType.Base);

			if (!_baseBranch.IsActive)
				yield break;

			var allCommonsAreActive = true;
			foreach (var commonBranch in _commonBranches)
			{
				allCommonsAreActive &= commonBranch.IsActive;
				yield return RetrieveModel(commonBranch, SpellType.Common);
			}

			if (allCommonsAreActive && _experienceBank.Level.Value > LevelRequiredForUlt)
				yield return RetrieveModel(_ultBranch, SpellType.Ult);
		}

		public IEnumerable<Skill> GetSkils()
		{
			yield return new Skill(_baseBranch, SpellType.Base);

			foreach (var commonBranch in _commonBranches)
				yield return new Skill(commonBranch, SpellType.Common);

			yield return new Skill(_ultBranch, SpellType.Ult);
		}

		private ISpellModel RetrieveModel(ISpellBranch branch, SpellType type)
		{
			return new SpellModelWrapper(branch.Model)
			{
				Class = Name,
				Type = type
			};
		}

		[Inject]
		private void Inject(DiContainer container, IExperienceBank experienceBank)
		{
			_experienceBank = experienceBank;
			container.InjectWhenNeeded(_baseBranch);
			foreach (var commonBranch in _commonBranches)
				container.InjectWhenNeeded(commonBranch);
			container.InjectWhenNeeded(_ultBranch);
		}

		public override IEnumerable<ISpellModel> GetMainModels()
		{
			throw new System.NotImplementedException();
		}

		public sealed class SpellModelWrapper : Classes.SpellModelWrapper
		{
			public SpellModelWrapper([NotNull] ISpellModel wrappedModel) : base(wrappedModel) { }

			public ClassName Class { get; init; }
			public SpellType Type { get; init; }
		}

		public enum SpellType
		{
			Base,
			Common,
			Ult,
			Buff,
			Empty
		}
	}
}