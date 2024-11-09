namespace Bones.Gameplay.Entities.Stats
{
    public struct EnemyStats : IStats
    {
        public float Health { get; set; }
        public float Harmony { get; set; }
        public float Damage { get; init; }
        public float Armor { get; init; }
        public float CriticalChance { get; init; }
        public float CriticalMultiplier { get; init; }
    }
}