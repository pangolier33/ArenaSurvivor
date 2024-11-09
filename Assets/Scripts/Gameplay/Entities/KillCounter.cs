using Bones.Gameplay.Events.Args;
using Bones.Gameplay.Meta;
using System.Linq;
using UniRx;

namespace Bones.Gameplay.Entities
{
	public class KillCounter
	{
		public int Count { get; private set; }

		public KillCounter(IMessageReceiver receiver, Level level)
		{
			level.Started += OnLevelStarted;
			receiver.Receive<EnemyDiedArgs>()
				.Select(x => x.Subject)
				.Subscribe(OnEnemyKilled);
		}

		private void OnLevelStarted()
		{
			Count = 0;
		}

		private void OnEnemyKilled(Enemy enemy)
		{
			Count++;
		}
	}
}
