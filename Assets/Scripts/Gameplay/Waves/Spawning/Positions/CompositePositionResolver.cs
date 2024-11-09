using System.Collections.Generic;
using System.Linq;
using Bones.Gameplay.Utils;
using UnityEngine;

namespace Bones.Gameplay.Waves.Spawning.Positions
{
    public sealed class CompositePositionResolver : IPositionResolver
    {
        private readonly IEnumerable<IPositionResolver> _resolvers;

        public CompositePositionResolver(params IPositionResolver[] positionResolvers)
        {
            _resolvers = positionResolvers;
        }

        public Vector2 PickUp()
        {
            return _resolvers.Select(x => x.PickUp()).Sum();
        }
    }
}