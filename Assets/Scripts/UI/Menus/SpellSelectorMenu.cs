using System;
using System.Collections.Generic;
using System.Linq;
using Bones.Gameplay.Experience;
using Bones.Gameplay.Meta.CharacterClases;
using Bones.Gameplay.Spells.Classes;
using Bones.Gameplay.Utils;
using Bones.UI.Bindings.Base;
using JetBrains.Annotations;
using Railcar.Time;
using UniRx;
using UnityEngine;
using Zenject;

namespace Bones.UI.Presenters.New
{
	public class SpellSelectorMenu : BaseBinder
	{
		[SerializeField] private AudioSource _sfx;
		[SerializeReference] private IBinding _levelBinding;

		[Header("Containers")]
		[SerializeField] private int _baseCount;
		[SerializeField] private ListViewModel<ISpellModel> _baseList;
		[SerializeField] private int _buffsCount;
		[SerializeField] private ListViewModel<ISpellModel> _buffsList;
		[SerializeField] private int _restCapacity;
		[SerializeField] private ListViewModel<ISpellModel> _restList;
        
		[Header("Selector")]
		[SerializeField] private ListViewModel<ISpellModel> _selectorList;
		[SerializeField] private int _selectorCapacity = 3;
        
		private ISpellContainer _generalContainer;
		private IExperienceBank _bank;
		private ITimeLord _timeLord;
		private ICharacterClassStorage _characterClassStorage;
		private IDisposable _subscription;
		private IReactiveCollection<ISpellModel> _baseCollection;
		private IReactiveCollection<ISpellModel> _buffCollection;
		private IReactiveCollection<ISpellModel> _restCollection;

		public event EventHandler<SpellSelectorMenu> Exited;
		
		public void Show(SpellSelectorRequestedArgs args)
		{
			_sfx.Play();
			_timeLord.DoesFlow = false;

			var selectorSpells = args.Container.GetModels()
												  .Where(x => x.OutBranch.IsAvailable)
												  .WeightedMix(_selectorCapacity, model => model.Weight)
												  .Select(model => new SpellModelWrapper(model, this));
			_selectorList.Bind(selectorSpells);

			var baseSpells = FilterModelsFromContainer(_baseCount, ClassContainer.SpellType.Base); 
			_baseCollection = _baseList.Bind(baseSpells);

			var buffSpells = FilterModelsFromContainer(_buffsCount, ClassContainer.SpellType.Buff);
			_buffCollection = _buffsList.Bind(buffSpells);

			var restSpells = FilterModelsFromContainer(_restCapacity, ClassContainer.SpellType.Common, ClassContainer.SpellType.Ult);
			_restCollection = _restList.Bind(restSpells);
			
			OnNext(args.Level, _levelBinding);
			_subscription = Disposable.Empty;
			
			IEnumerable<ISpellModel> FilterModelsFromContainer(int capacity, params ClassContainer.SpellType[] types)
			{
				var nonEmpty = _generalContainer.GetMainModels()
												.Where(x => x.OutBranch.IsActive)
												.Where(x => types.Contains(x.Type()))
												.ToArray();
				var empty = Enumerable.Range(0, Math.Clamp(capacity - nonEmpty.Length, 0, capacity))
									  .Select(_ => SpellModelExtensions.Empty());
				return nonEmpty.Concat(empty);
			}
		}

		private void OnSpellApplied(ISpellModel model)
		{
			Process(_baseCollection, model, ClassContainer.SpellType.Base);
			Process(_buffCollection, model, ClassContainer.SpellType.Buff);
			Process(_restCollection, model, ClassContainer.SpellType.Common, ClassContainer.SpellType.Ult);
			
			_timeLord.DoesFlow = true;
			Exited?.Invoke(this, this);
		}

		private static void Process(IList<ISpellModel> collection, ISpellModel model, params ClassContainer.SpellType[] types)
		{
			if (!types.Contains(model.Type()))
				return;
			if (collection.Contains(model))
				return;
			
			for (var i = 0; i < collection.Count; i++)
			{
				if (!collection[i].IsEmpty())
					continue;
				collection[i] = model;
				return;
			}
			
			collection.Add(model);
		}

		private void OnDestroy()
		{
			_subscription.Dispose();
		}

		[Inject]
		private void Inject(ISpellContainer generalContainer, 
			IExperienceBank bank, 
			ITimeLord timeLord,
			ICharacterClassStorage characterClassStorage)
		{
			_generalContainer = generalContainer;
			_bank = bank;
			_timeLord = timeLord;
			_characterClassStorage = characterClassStorage;
		}
        
		private class SpellModelWrapper : Gameplay.Spells.Classes.SpellModelWrapper
		{
			private readonly SpellSelectorMenu _menu;
            
			public SpellModelWrapper([NotNull] ISpellModel wrappedModel, SpellSelectorMenu menu) : base(wrappedModel)
			{
				_menu = menu;
			}

			public override IDisposable Apply()
			{
				var result = base.Apply();
				_menu.OnSpellApplied(this);
				return result;
			}
		}
	}
}