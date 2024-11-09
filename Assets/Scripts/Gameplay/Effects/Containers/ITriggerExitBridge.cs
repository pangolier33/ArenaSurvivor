using UnityEngine;

namespace Bones.Gameplay.Effects.Containers
{
	public interface ITriggerExitBridge
	{
		void OnOut(GameObject component);
	}
}