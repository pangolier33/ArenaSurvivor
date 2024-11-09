using UnityEngine;

namespace Bones.Gameplay.Inputs
{
    public interface IDirectionalInput
    {
        public Vector2 Direction { get; }
		public float JoySpeedModifier { get; }
    }
}