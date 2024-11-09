using Railcar.Dice;
using Zenject;

namespace Bones.Gameplay.Entities.Stats
{
    public sealed class EnemyStatsProcessor : BaseStatsProcessor<EnemyStats>
    {
        private EnemyStatsProcessor(
            [Inject] IDice dice,
            EnemyStats stats
        ) : base(dice, stats)
        {
        }
    }
}