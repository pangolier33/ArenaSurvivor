using Bones.Gameplay.Effects.Provider;

namespace Bones.Gameplay.Effects.Containers.Projectiles
{
    public struct ProjectileSettings
    {
        public IMutableTrace Trace { get; set; }
        public IEffect OnSpawnedEffect { get; init; }
        public IEffect OnTriggeredEffect { get; init; }
    }
}