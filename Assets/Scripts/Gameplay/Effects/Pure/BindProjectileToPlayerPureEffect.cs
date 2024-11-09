using System;
using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Effects.Pure;
using Bones.Gameplay.Players;
using UnityEngine;
using Zenject;

namespace Bones.Gameplay.Effects.Containers.Projectiles
{
    [Serializable]
    public sealed class BindProjectileToPlayerPureEffect : PureEffect
    {
        private Transform _playerTransform;
        
        protected override void Invoke(ITrace trace)
        {
            var handle = trace.Get<IProjectileHandle>();
            var handleTransform = handle.Instance.transform;
            handleTransform.parent = _playerTransform;
            handleTransform.localPosition = Vector3.zero;
        }

        [Inject]
        private void Inject(Player player)
        {
            _playerTransform = player.transform;
        }
    }
}