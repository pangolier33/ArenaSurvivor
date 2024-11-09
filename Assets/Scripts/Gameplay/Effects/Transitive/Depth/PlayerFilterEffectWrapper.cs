using System;
using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Players;
using Zenject;

namespace Bones.Gameplay.Effects.Transitive.Depth
{
	[Serializable]
    public class PlayerFilterEffectWrapper : TransitiveEffect
    {
        private Player _player;
        
        protected override void Invoke(IMutableTrace trace, IEffect next)
        {
            next.Invoke(trace.Add(_player));
        }

        [Inject]
        private void Inject(Player player)
        {
            _player = player;
        }
    }
}