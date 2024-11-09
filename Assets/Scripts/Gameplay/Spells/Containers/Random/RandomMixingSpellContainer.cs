using System.Collections.Generic;
using System.Linq;
using Bones.Gameplay.Spells.Classes;
using UnityEngine;
using Zenject;

namespace Bones.Gameplay.Spells.Containers.Random
{
	[CreateAssetMenu]
	public sealed class RandomMixingSpellContainer : BaseSpellContainer
	{
		[SerializeField] private BaseSpellBranch[] _branches;

		public override IEnumerable<ISpellModel> GetMainModels()
		{
			return _branches.Select(x => x.Model);
		}

		public override IEnumerable<ISpellModel> GetModels()
		{
			return _branches.Select(x => x.Model);
		}

		[Inject]
		private void Inject(DiContainer container)
		{
			foreach (var branch in _branches)
				container.InjectWhenNeeded(branch);
		}
	}
}