using System.Collections.Generic;
using Bones.Gameplay.Items;
using Bones.Gameplay.Players;
using UnityEngine;

namespace Bones.Gameplay.Flocks.Pulling
{
	public class ItemsContainer : PullingContainer<HashSet<Item>, Item>
	{
		protected ItemsContainer(Vector2Int extents, float clusterSize, Player player, IEnumerable<Item> entities, ItemCollectingSettings settings)
			: base(extents, clusterSize, player.transform, entities, () => settings.MinimalPullingDistance)
		{
		}
	}
}