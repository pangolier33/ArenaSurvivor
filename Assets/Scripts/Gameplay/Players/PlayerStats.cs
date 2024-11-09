using Bones.Gameplay.Entities.Stats;

namespace Bones.Gameplay.Players
{
    public struct PlayerStats : IStats
    {
        public float Health { get; set; }
        public float Harmony { get; set; }
        public float MaxHealth { get; set; }
        public float HealthRegenDelay { get; set; }
        public float HealthRegenAmount { get; set; }
        
        
        public float Shield { get; set; }
        public float MaxShield { get; set; }
        public float ShieldRegenDelay { get; set; }
        public float ShieldRegenAmount { get; set; }
        
        public float Damage { get; init; }
        public float Armor { get; init; }
        public float CriticalChance { get; init; }
        public float CriticalMultiplier { get; init; }
        
        public float MulticastChance { get; init; }
        public float CastSpeed { get; init; }
    }
}