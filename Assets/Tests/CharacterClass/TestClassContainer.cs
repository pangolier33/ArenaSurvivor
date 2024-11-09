using Bones.Gameplay.Meta.CharacterClases;
using Bones.Gameplay.Spells.Classes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Tests
{
	public class TestClassContainer : IClassContainer
	{
		private IEnumerable<Skill> _skills;

		public ClassName Name { get; init; }

		public Sprite Icon { get; init; }

		public IEnumerable<Skill> GetSkils()
		{
			return _skills;
		}

		public IEnumerable<ISpellModel> GetModels()
		{
			return null;
		}

		public TestClassContainer(IEnumerable<Skill> skills)
		{
			_skills = skills.ToArray();
		}
	}
}
