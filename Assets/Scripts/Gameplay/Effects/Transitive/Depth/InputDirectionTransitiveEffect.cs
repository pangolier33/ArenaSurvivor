using System;
using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Inputs;
using UniRx;
using UnityEngine;
using Zenject;

namespace Bones.Gameplay.Effects.Transitive.Depth
{
	[Serializable]
	public class InputDirectionTransitiveEffect : SetVariableEffectWrapper<Vector2>
	{
		private IReadOnlyReactiveProperty<Vector2> _direction;

		protected override Vector2 GetValue(ITrace provider)
		{
			return _direction.Value;
		}

		[Inject]
		private void Inject(InputObservable input)
		{
			_direction = input.NotNullDirection;
		}
	}
}