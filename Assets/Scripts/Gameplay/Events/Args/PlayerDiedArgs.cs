using Bones.Gameplay.Players;

namespace Bones.Gameplay.Events.Args
{
	public readonly struct PlayerDiedArgs : IMessageArgs
	{
		public Player Player { get; init; }
	}
}
