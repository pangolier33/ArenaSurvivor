using System;
using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Effects.Provider.Branches;

namespace Bones.Gameplay.Effects.Transitive.Depth
{
	[Serializable]
	public class CreateBranchDTEC : AddTEC<IBranch>
	{
		protected override IBranch RetrieveArg(ITrace trace)
		{
			var branch = new Branch();
			trace.ConnectToClosest(branch);
			return branch;
		}
	}
}