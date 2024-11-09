using Bones.Gameplay.Waves.Spawning.Positions;
using UnityEngine;

namespace Bones.Gameplay.Waves
{
	[CreateAssetMenu(menuName = "Waves/Spawner Shared Config", fileName = "SpawnerSharedConfig")]
	public class SpawnerSharedConfig : ScriptableObject
	{
		public float DespawningDistance;
		public PositionOriginName Origin;
		[SerializeReference] public IPositionResolver PositionResolver = new InCirclePositionResolver();
	}
}