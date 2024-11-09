using System;
using Bones.Gameplay.Waves.Spawning.Positions.Origins;
using Zenject;

namespace Bones.Gameplay.Waves.Spawning.Positions
{
    public class PositionResolverFactory : IFactory<PositionOriginName, IPositionResolver, IPositionResolver>
    {
        private readonly WorldPositionResolver _worldPositionResolver;
        private readonly LocalPositionResolver _localPositionResolver;

        public PositionResolverFactory(
            [Inject] WorldPositionResolver worldPositionResolver,
            [Inject] LocalPositionResolver localPositionResolver
            )
        {
            _worldPositionResolver = worldPositionResolver;
            _localPositionResolver = localPositionResolver;
        }
        
        public IPositionResolver Create(PositionOriginName origin, IPositionResolver resolver)
        {
            return new CompositePositionResolver(
                origin switch {
                    PositionOriginName.World => _worldPositionResolver,
                    PositionOriginName.LocalToPlayer => _localPositionResolver,
                    _ => throw new ArgumentOutOfRangeException(nameof(origin), origin, null)
                },
                resolver
                );
        }
    }
}