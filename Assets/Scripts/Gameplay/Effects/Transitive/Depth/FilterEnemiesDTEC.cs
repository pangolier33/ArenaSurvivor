using System;
using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Entities;
using Bones.Gameplay.Players;
using Bones.Gameplay.Stats.Containers.Graphs;
using UnityEngine;
using Zenject;

namespace Bones.Gameplay.Effects.Transitive.Depth
{
	[Serializable]
	public sealed class FilterEnemiesDTEC : MutationTransitiveEffect
	{
		[SerializeField] private StatName _distanceLimitName;
		private EnemiesCollectionCache _enemies;
        
		protected override object RetrieveArg(ITrace trace)
		{
			return _enemies.GetSorted(trace.Get<ISpellActor>()).GetEnumerator();
		}

		[Inject]
		private void Inject(EnemiesCollectionCache enemies)
		{
			_enemies = enemies;
		}
	}
}