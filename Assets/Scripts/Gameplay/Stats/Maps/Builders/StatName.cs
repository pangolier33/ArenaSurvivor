namespace Bones.Gameplay.Stats.Containers.Graphs
{
	public enum StatName : byte
	{
		Health,
		MaxHealth,
		
		Shield,
		MaxShield,
		
		Armor,
		Harmony,
		
		Damage,
		CriticalChance,
		CriticalMultiplier,
		MovingSpeed,
		
		Multicast,
		Count,
		Size,
		Cooldown,
		Piercing,
		ProjectileSpeed,
		Distance,
		Duration,
		
		SpawnPosition,
		TargetPosition,
		Dummy,
		
		OverallHealth,
		AttackDelay,
		Delta,
		Area,
		HealthPercent,
		Cap,
		NameTag,
		HealthRegen,
		ShieldRegen,
		Thorns,
		Multiplier,
		//used only for spells that require self-damage (thorns),
		//otherwise, player is gonna be killed one shot with enemy stat boost
		FakeDamage,
		Impulse,

		Stamina,
		StaminaParametrs,
		StaminaZeroReached
	}
}