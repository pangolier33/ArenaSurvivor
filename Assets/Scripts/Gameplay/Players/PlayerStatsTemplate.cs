using System;
using UnityEngine;

namespace Bones.Gameplay.Players
{
    [Serializable]
    public class PlayerStatsTemplate
    {
        [SerializeField] private float _health;
        [SerializeField] private float _harmony;
        [SerializeField] private float _healthRegenDelay;
        [SerializeField] private float _healthRegenAmount;
        
        [SerializeField] private float _shield;
        [SerializeField] private float _shieldRegenDelay;
        [SerializeField] private float _shieldRegenAmount;

        [SerializeField] private float _damage;
        [SerializeField] private float _armor;
        [SerializeField] private float _criticalChance;
        [SerializeField] private float _criticalMultiplier;

        [SerializeField] private float _multicastChance;
        [SerializeField] private float _castSpeed;

        public PlayerStats Build()
        {
            return new PlayerStats
            {
                Health = _health,
                MaxHealth = _health,
                Harmony = 1f + _harmony / 100f,
                HealthRegenDelay = _healthRegenDelay,
                HealthRegenAmount = _healthRegenAmount,
                
                Shield = _shield,
                MaxShield = _shield,
                ShieldRegenDelay = _shieldRegenDelay,
                ShieldRegenAmount = _shieldRegenAmount,
                
                Damage = _damage,
                Armor = 1f - _armor / 100f,
                CriticalChance = _criticalChance / 100f,
                CriticalMultiplier = 1 + _criticalMultiplier / 100f,
                
                CastSpeed = _castSpeed,
                MulticastChance = _multicastChance / 100f
            };
        }
    }
}