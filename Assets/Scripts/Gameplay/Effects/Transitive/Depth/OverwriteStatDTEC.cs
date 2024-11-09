using System;
using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Stats.Containers;
using UnityEngine;

namespace Bones.Gameplay.Effects.Transitive.Depth
{
	[Serializable]
	public class OverwriteStatDTEC : InjectStatDTEC
	{
		[SerializeField] private bool _isCertain;
		[SerializeReference] private IStatBuilder _builder;

		protected override IStat RetrieveStat(ITrace trace) => _isCertain switch
		{
			true => _builder.Build(),
			false => _builder.BuildUncertain()
		};
	}
}