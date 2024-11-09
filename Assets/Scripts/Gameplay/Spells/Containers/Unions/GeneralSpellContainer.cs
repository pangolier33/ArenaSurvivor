using System.Collections.Generic;
using System.Linq;
using Bones.Gameplay.Events.Callbacks.Templates.Base;
using Bones.Gameplay.Meta.CharacterClases;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Bones.Gameplay.Spells.Classes
{
	[CreateAssetMenu]
	public sealed class GeneralSpellContainer : BaseSpellContainer, IInjectable, IInitializable
	{
		// todo: add validation
		[InlineEditor]
		[InlineProperty]
		[SerializeReference]
		private ISpellBranch _initial;
			
		[InlineEditor]
		[SerializeField] 
		private ClassContainer[] _classes;
		[SerializeField] 
		private int _maxClassSpellsCount;
		
		[InlineEditor]
		[SerializeReference]
		private ISpellBranch[] _buffs;
		[SerializeField]
		private int _maxBuffsCount;
		
		[InlineEditor]
		[SerializeReference]
		private ISpellBranch[] _fallbackBuffs;
		private ICharacterClassStorage _characterClassStorage;

		void IInitializable.Initialize()
		{
			// todo: move to handler
			_initial.Model.Apply();
		}
		
		public override IEnumerable<ISpellModel> GetModels()
		{
			var mainModels = GetMainModels().Where(x => x.OutBranch.IsAvailable).ToArray();
			return mainModels.Length > 0 ? mainModels : GetFallbackModels();
		}
		
		public override IEnumerable<ISpellModel> GetMainModels()
		{
			var classSpells = _characterClassStorage.ActiveClasses.SelectMany(x => x.ClassContainer.GetModels());
			//var classSpells = _classes.SelectMany(@class => @class.GetModels()).ToArray();
			if (classSpells.Count(x => x.OutBranch.IsActive && x.Type() != ClassContainer.SpellType.Base) >= _maxClassSpellsCount)
				classSpells = classSpells.Where(x => x.OutBranch.IsActive).ToArray();

			var buffs = _buffs.Select(x => x.Model).ToArray();
			if (buffs.Count(x => x.OutBranch.IsActive) >= _maxBuffsCount)
				buffs = buffs.Where(x => x.OutBranch.IsActive).ToArray();
			
			return classSpells.Concat(buffs);
		}

		private IEnumerable<ISpellModel> GetFallbackModels()
		{
			return _fallbackBuffs.Where(x => x.IsAvailable).Select(x => x.Model);
		}

		[Inject]
		private void Inject(DiContainer container, ICharacterClassStorage characterClassStorage)
		{
			_characterClassStorage = characterClassStorage;
			container.InjectWhenNeeded(_initial);
			foreach (var @class in _classes)
				container.InjectWhenNeeded(@class);
			foreach (var buff in _buffs)
				container.InjectWhenNeeded(buff);
			foreach (var fallbackBuff in _fallbackBuffs)
				container.InjectWhenNeeded(fallbackBuff);
		}
	}
}