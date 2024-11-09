using System;
using System.Collections;
using Bones.Gameplay.Effects.Provider;

namespace Bones.Gameplay.Effects.Transitive.Width
{
	[Serializable]
	public class RetrieveUnitWTEC : TransitiveEffect
	{
		protected override void Invoke(IMutableTrace trace, IEffect next)
		{
			var enumerator = trace.Get<IEnumerator>();
			if (enumerator.MoveNext())
				next.Invoke(trace.Add(enumerator.Current));
		}
	}
}