using System;
using Bones.Gameplay.Spells.Classes;
using Bones.UI.Bindings.Base;
using UniRx;
using UnityEngine;

namespace Bones.UI.Presenters.New
{
	public class SpellViewModel : BaseViewModel<ISpellModel>
	{
		[SerializeReference] private IBinding _iconBinding;
		[SerializeReference] private IBinding _nameBinding;
		[SerializeReference] private IBinding _descriptionBinding;
        
		[SerializeReference] private IBinding _typeBinding;
		[SerializeReference] private IBinding _classBinding;
        
		[SerializeReference] private IBinding _callbackBinding;
		
		[SerializeReference] private IBinding _levelBinding;
		[SerializeReference] private IBinding _maxLevelBinding;
		[SerializeReference] private IBinding _levelProportionBinding;
        
		protected override IDisposable SetupBindings(ISpellModel model)
		{
			OnNext(model.Name, _nameBinding);
			OnNext(model.Icon, _iconBinding);
			OnNext(model.Description, _descriptionBinding);
            
			OnNext((int)model.Type(), _typeBinding);
			OnNext((int)model.Class(), _classBinding);
			OnNext<Action>(() => model.Apply(), _callbackBinding);
			
			OnNext(model.MaxLevel(), _maxLevelBinding);
			OnNext(model.LevelValue() / (float)model.MaxLevel(), _levelProportionBinding);
			var level = model.LevelProperty();
			if (level != null)
				return Bind(level, _levelBinding);
			OnNext(0, _levelBinding);
			return Disposable.Empty;
		}
	}
}