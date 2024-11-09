using Bones.Gameplay.Players;
using Railcar.Time;
using Zenject;

namespace Bones.Gameplay.Map
{
	public class PlayerPositionTracker : PositionTracker
	{
		public PlayerPositionTracker([Inject] Player player, [Inject(Id = TimeID.Fixed)] IStopwatch stopwatch)
			: base(player.transform, stopwatch)
		{
		}
	}
}