using System;
using UnityEngine;

namespace Bones.Gameplay.Spells.Classes
{
	public interface ISpellModel
	{
		string Name { get; }
		string Description { get; }
		Sprite Icon { get; }
		float Weight { get; }
		IDisposable Apply();
		ISpellBranch OutBranch { get; }
	}
}