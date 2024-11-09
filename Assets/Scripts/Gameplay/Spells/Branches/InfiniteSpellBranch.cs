using System;
using UniRx;
using UnityEngine;

namespace Bones.Gameplay.Spells.Classes
{
	[CreateAssetMenu]
	public sealed class InfiniteSpellBranch : CustomMapSpellBranch
	{
		[SerializeField] private float _weight;
		
		private readonly CompositeDisposable _subscriptions = new();
		private bool _isUsed;
		
		public override bool IsActive => _isUsed;
		public override bool IsAvailable => true;
		public override float Weight => _weight;

		
		protected override IDisposable ApplyWhenAvailable()
		{
			base.ApplyWhenAvailable().AddTo(_subscriptions);
			_isUsed = true;
			return _subscriptions;
		}

		protected override void OnReset()
		{
			_isUsed = false;
		}
	}
}