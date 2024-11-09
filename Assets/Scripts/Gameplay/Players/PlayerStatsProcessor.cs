using System;
using Bones.Gameplay.Entities.Stats;
using Bones.Gameplay.Utils;
using Railcar.Dice;
using Railcar.Time;
using UniRx;
using Zenject;

namespace Bones.Gameplay.Players
{
    public sealed class PlayerStatsProcessor : BaseStatsProcessor<PlayerStats>, IDisposable
    {
        private readonly IMessagePublisher _publisher;
        private readonly CompositeDisposable _subscriptions = new();
        
        public Player Player { get; set; }
        
        public event EventHandler<float> HealthChanged;
        public event EventHandler<float> ShieldChanged;
        
        public PlayerStatsProcessor(
            [Inject] IMessagePublisher publisher,
            [Inject] IDice dice,
            [Inject(Id = TimeID.Fixed)] ITimer timer,
            PlayerStats stats
        ) : base(dice, stats)
        {
            _publisher = publisher;
            timer.MarkAndRepeat(Stats.HealthRegenDelay, OnSelfHealing).AddTo(_subscriptions);
            timer.MarkAndRepeat(Stats.ShieldRegenDelay, OnSelfShielding).AddTo(_subscriptions);
        }

        public float Health => Stats.Health;
        public float MaxHealth => Stats.MaxHealth;
        public float Shield => Stats.Shield;
        public float MaxShield => Stats.MaxShield;
        public float MulticastChance => Stats.MulticastChance;
        public float CastSpeed => Stats.CastSpeed;

        public override void TakeDamage(float incomingDamage)
        {
            var cutDamage = incomingDamage * Stats.Armor;
            
            if (Stats.Shield == 0)
                goto HealthSubtracting;
            
            if (Stats.Shield < cutDamage)
            {
                Stats.Shield = 0;
                ShieldChanged?.Invoke(this, 0);
            }
            else
            {
                var newShield = Stats.Shield - cutDamage;
                Stats.Shield = newShield;
                ShieldChanged?.Invoke(this, newShield);
                _publisher.Publish(new PlayerDamagedArgs { Position = Player.Position });
                cutDamage = 0;
            }
            
            HealthSubtracting:
            if (Stats.Health != 0 && cutDamage > 0)
            {
                _publisher.Publish(new PlayerDamagedArgs { Position = Player.Position });
                if (Stats.Health < cutDamage)
                {
                    Stats.Health = 0;
                    HealthChanged?.Invoke(this, 0);
                }
                else
                {
                    var newHealth = Stats.Health - cutDamage;
                    Stats.Health = newHealth;
                    HealthChanged?.Invoke(this, newHealth);
                }
            }

            OnDamaged();
        }

        public override void Heal(float amount)
        {
            var cutHeal = amount * Stats.Harmony;

            Stats.Health += cutHeal;
            if (Stats.Health >= Stats.MaxHealth)
            {
                cutHeal = Stats.Health - Stats.MaxHealth;
                Stats.Health = Stats.MaxHealth;
                HealthChanged?.Invoke(this, Stats.MaxHealth);
            }
            else
            {
                HealthChanged?.Invoke(this, Stats.Health);
                return;
            }

            Stats.Shield = Math.Min(Stats.Shield + cutHeal, MaxShield);
            ShieldChanged?.Invoke(this, Stats.Shield);
        }

        public void Dispose()
        {
            _subscriptions.Dispose();
        }
        
        private void OnSelfHealing(float f)
        {
            Stats.Health = Math.Min(Stats.Health + Stats.HealthRegenAmount, Stats.MaxHealth);
            HealthChanged?.Invoke(this, Stats.Health);
        }

        private void OnSelfShielding(float obj)
        {
            Stats.Shield = Math.Min(Stats.Shield + Stats.ShieldRegenAmount, Stats.MaxShield);
            ShieldChanged?.Invoke(this, Stats.Shield);
        }
    }
}