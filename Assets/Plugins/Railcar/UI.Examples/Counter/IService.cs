using System;
using UnityEngine;

namespace Railcar.UI
{
	public interface IService
	{
		Sprite Icon { get; }
		IObservable<int> Value { get; }
		void Update();
	}
}