using System;
using UnityEngine;

namespace Bones.Gameplay.Waves.Spawning.Amounts
{
    [Serializable]
    public class ConstantAmountResolver : IAmountResolver
    {
        [SerializeField] private int _amount;
        
        public int GetAmount()
        {
            return _amount;
        }
    }
}