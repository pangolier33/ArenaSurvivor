using System;
using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Inputs;
using Bones.Gameplay.Stats.Containers;
using Zenject;

namespace Bones.Gameplay.Effects.Transitive.Depth
{
	[Serializable]
	public class CacheInputDirectionDTEC : InjectStatDTEC
	{
		private InputObservable _input;
		
		protected override IStat RetrieveStat(ITrace trace)
		{
			return new ReadOnlyStat<Pair>(_input.NotNullDirection.Value);
		}

		[Inject]
		private void Inject(InputObservable input)
		{
			_input = input;
		}
	}
}