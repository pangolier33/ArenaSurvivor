using System;
using System.Collections.Generic;
using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Entities;
using UnityEngine;
using Zenject;

namespace Bones.Gameplay.Effects.Transitive.Depth
{
	[Serializable]
    public sealed class EnemiesFilterEffectWrapper : TransitiveEffect
    {
        [SerializeField] private float _maxDistance;
        [SerializeField] private string _centerId;

        private IEnumerable<Enemy> _enemies;

        protected override void Invoke(IMutableTrace trace, IEffect next)
        {
            throw new NotImplementedException();
            /*var center = mutableTrace.GetVariable<Vector2>(_centerId);
            next.Invoke(mutableTrace.Add(_enemies.Where(x => x is not CircleOfExperience).Where(x => (center - x.Position).magnitude < _maxDistance)
                                                     .OrderBy(x => (center - x.Position).magnitude)
            ));*/
        }

        [Inject]
        private void Inject(IEnumerable<Enemy> enemies)
        {
            _enemies = enemies;
        }
    }
}