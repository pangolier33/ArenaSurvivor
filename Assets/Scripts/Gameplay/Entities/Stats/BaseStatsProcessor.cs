using System;
using Bones.Gameplay.Effects.Transitive.Depth;
using Bones.Gameplay.Stats.Containers.Graphs;
using Railcar.Dice;

namespace Bones.Gameplay.Entities.Stats
{
    public abstract class BaseStatsProcessor<T> : IStatsProcessor, INewStats
        where T : IStats
    {
        // not a property cause used as value type
        protected T Stats;
        private readonly float _maxHealth;
        
        public event EventHandler Damaged;
        public event EventHandler Died;
        
        protected BaseStatsProcessor(
            IDice dice,
            T stats
        )
        {
            Dice = dice;
            Stats = stats;
            _maxHealth = stats.Health;
        }

        protected IDice Dice { get; }

        public void Subtract(StatName name, float value)
        {
            switch (name)
            {
                case StatName.Damage:
                    break;
            }
        }

        public virtual void TakeDamage(float incomingDamage)
        {
            var cutDamage = incomingDamage * Stats.Armor;
            Stats.Health -= cutDamage;

            OnDamaged();
            if (Stats.Health <= 0)
                Kill();
        }

        protected void OnDamaged()
        {
            Damaged?.Invoke(this, EventArgs.Empty);
        }

        public void Kill()
        {
            Stats.Health = 0;
            Died?.Invoke(this, EventArgs.Empty);
        }

        public virtual void Heal(float amount)
        {
            var cutHeal = amount * Stats.Harmony;
            Stats.Health = Math.Min(Stats.Health + cutHeal, _maxHealth);
        }

        public virtual float DealDamage()
        {
            var damage = Stats.Damage;
            
            if (Dice.Roll(Stats.CriticalChance))
                damage *= Stats.CriticalMultiplier;
            
            return damage;
        }

        public float Get(StatName name)
        {
            return name switch
            {
                _ => throw new ArgumentOutOfRangeException(nameof(name), name, null)
            };
        }

        public void Add(StatName name, float value)
        {
            
        }
    }
}