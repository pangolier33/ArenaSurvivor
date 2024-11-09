using System;
using UniRx;
using UnityEngine;

namespace Railcar.UI
{
	public class Service : IService
	{
		private readonly ReactiveProperty<int> _value = new();
		
		public Service(Sprite icon)
		{
			Icon = icon;
		}

		public Sprite Icon { get; }
		public IObservable<int> Value => _value;

		public void Update() => _value.Value++;
	}
}