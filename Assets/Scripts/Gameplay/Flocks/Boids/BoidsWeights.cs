using System;
using UnityEngine;

namespace Bones.Gameplay.Flocks.Boids
{
    [Serializable]
    public struct BoidsWeights
    {
        [SerializeField] private float _separation;
        [SerializeField] private float _emergencySeparation;
        [SerializeField] private int _countLimiter;
        [Space]
        [SerializeField] private float _cohesion;
        [Space]
        [SerializeField] private float _targeting;
        [Space]
        [SerializeField] private float _alignment;

        public float Separation => _separation;
        public float EmergencySeparation => _emergencySeparation;
        public int CountLimiter => _countLimiter;
        public float Cohesion => _cohesion;
        public float Targeting => _targeting;
        public float Alignment => _alignment;
    }
}