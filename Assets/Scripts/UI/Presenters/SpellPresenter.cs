using Bones.Gameplay.Spells;
using Bones.Gameplay.Spells.Classes;
using Bones.UI.Bindings.Base;
using UnityEngine;

namespace Bones.UI.Presenters
{
    public class SpellPresenter : BasePresenter
    {
        [SerializeReference] private IBinding _iconBinding;
        [SerializeReference] private IBinding _fillingBinding;
        [SerializeReference] private IBinding _classBinding;
        [SerializeReference] private IBinding _spellTypeBinding;

        public void Subscribe(ISpellModel spell)
        {
            OnNext(spell.Icon, _iconBinding);
            OnNext((int)spell.Class(), _classBinding);
            OnNext((int)spell.Type(), _spellTypeBinding);
            OnNext((float)spell.LevelValue() / spell.MaxLevel(), _fillingBinding);
        }
    }
}
