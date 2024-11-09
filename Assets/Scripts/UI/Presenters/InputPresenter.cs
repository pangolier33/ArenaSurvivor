using System;
using Bones.Gameplay.Inputs;
using UnityEngine;
using Zenject;

namespace Bones.UI.Presenters
{
	public sealed class InputPresenter : InjectedPresenter<Vector2>
	{
		private IObservable<Vector2> _model;
		protected override IObservable<Vector2> RetrieveModel() => _model;
		
		[Inject]
		private void Inject(InputObservable inputObservable)
		{
			_model = inputObservable.NotNullDirection;
		}
	}
}