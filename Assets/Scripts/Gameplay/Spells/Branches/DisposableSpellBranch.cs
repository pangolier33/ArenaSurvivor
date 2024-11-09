using System;
using UnityEngine;

namespace Bones.Gameplay.Spells.Classes
{
	[CreateAssetMenu]
	public sealed class DisposableSpellBranch : CustomMapSpellBranch
	{
		[SerializeField] private float _weight;
		private bool _isUsed;
		
		public override bool IsActive => _isUsed;
		public override bool IsAvailable => !IsActive;
		public override float Weight => _weight;

		protected override IDisposable ApplyWhenAvailable()
		{
			var subscription = base.ApplyWhenAvailable();
			_isUsed = true;
			return subscription;
		}

		protected override void OnReset()
		{
			_isUsed = false;
		}
	}
}