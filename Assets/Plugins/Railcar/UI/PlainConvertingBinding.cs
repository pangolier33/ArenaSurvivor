using System;
using UnityEngine;

namespace Railcar.UI
{
	public abstract class PlainConvertingBinding<TFrom, TTo> : IBinding, IObserver<TFrom>
	{
		[SerializeField] private UnitBindingBuilder _binding;
		
		public void OnCompleted()
		{
			
		}

		public void OnError(Exception error)
		{
			throw error;
		}

		public void OnNext(TFrom value)
		{
			_binding.Build<TTo>().OnNext(Convert(value));
		}

		protected abstract TTo Convert(TFrom source);
	}
}