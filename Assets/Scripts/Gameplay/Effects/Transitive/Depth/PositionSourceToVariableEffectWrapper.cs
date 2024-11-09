using System;
using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Players;
using UnityEngine;

namespace Bones.Gameplay.Effects.Transitive.Depth
{
    [Serializable]
    public sealed class PositionSourceToVariableEffectWrapper : SetVariableEffectWrapper<Vector2>
    {
        protected override Vector2 GetValue(ITrace provider)
        {
            return provider.Get<ISpellActor>().Position;
        }
    }
}
