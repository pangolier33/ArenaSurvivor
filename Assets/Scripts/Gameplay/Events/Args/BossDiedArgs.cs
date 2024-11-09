namespace Bones.Gameplay.Events.Args
{
    public readonly struct BossDiedArgs : IMessageArgs
	{ 
		public bool MiniBoss { get; init; }
    }
}