namespace Bones.Gameplay.Entities.Stats
{
    public interface IStatsProcessor
    {
        void TakeDamage(float incomingDamage);
        float DealDamage();
    }
}