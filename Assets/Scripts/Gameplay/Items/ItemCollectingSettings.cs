using System;
using UnityEngine;

namespace Bones.Gameplay.Items
{
    [Serializable]
    public class ItemCollectingSettings
    {
        [field: SerializeField] public float DespawningDistance { get; set; }
        [field: SerializeField] public float CollectingDistance { get; set; }
        [field: SerializeField] public float PullingForce { get; set; }
        [field: SerializeField] public float MinimalPullingDistance { get; set; }
		[field: SerializeField] public AnimationCurve PullingCurve { get; set; }
    }
}