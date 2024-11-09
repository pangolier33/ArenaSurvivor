using Bones.Gameplay.Items;

namespace Bones.Gameplay.Events.Args
{
    public readonly struct ItemCollectedArgs : IMessageArgs
    {
        public ItemTag Tag { get; init; }
    }
}