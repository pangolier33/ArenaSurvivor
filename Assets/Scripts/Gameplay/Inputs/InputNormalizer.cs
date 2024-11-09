using UnityEngine;

namespace Bones.Gameplay.Inputs
{
    public class InputNormalizer : IDirectionalInput
    {
        private readonly IDirectionalInput _wrappedInput;

        public InputNormalizer(IDirectionalInput wrappedInput)
        {
            _wrappedInput = wrappedInput;
        }
        
        public Vector2 Direction => _wrappedInput.Direction.normalized;

		public float JoySpeedModifier => _wrappedInput.JoySpeedModifier;
    }
}