using System;
using UnityEngine;

namespace Bones.Gameplay.Factories.Pools
{
	[Serializable]
	public struct AudioInfo
	{
		[field: SerializeField] public float Volume { get; private set; }
		[field: SerializeField] public AudioClip Clip { get; private set; }
		[field: SerializeField] public bool Spatialize { get; private set; }
		[field: SerializeField] public float SpatialBlend { get; private set; }
		[field: SerializeField] public float Pitch { get; private set; }
	}
}