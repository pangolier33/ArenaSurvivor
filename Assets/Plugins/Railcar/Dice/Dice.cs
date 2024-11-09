using UnityEngine;

namespace Railcar.Dice
{
    public class Dice : IDice
    {
        public float Get() => Random.value;
    }
}