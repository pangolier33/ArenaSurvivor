using System.Linq;
using UnityEngine;

namespace Bones.Gameplay.Waves
{
    [CreateAssetMenu(menuName = "Utils/Animation Curve", fileName = "New Curve")]
    public class ScriptableCurve : ScriptableObject
    {
		private const int SecondsInMinute = 60;

        [SerializeField] private AnimationCurve _animationCurve;
		[SerializeField] private float _oneMinuteScale = 0.1f;

        public float Evaluate(float time)
        {
			float convertedTime = time / SecondsInMinute * _oneMinuteScale;
			convertedTime = Mathf.Min(convertedTime, _animationCurve.keys.Last().time);
            return _animationCurve.Evaluate(convertedTime);
        }
    }
}