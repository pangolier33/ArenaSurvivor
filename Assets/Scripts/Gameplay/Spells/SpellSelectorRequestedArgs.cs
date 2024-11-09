using Bones.Gameplay.Events.Args;
using Bones.Gameplay.Spells.Classes;

namespace Bones.UI.Presenters.New
{
	public readonly struct SpellSelectorRequestedArgs : IMessageArgs
    {
        public ISpellContainer Container { get; init; }
        public int Level { get; init; }
    }
}