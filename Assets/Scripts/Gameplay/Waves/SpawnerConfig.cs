using System;
using Bones.Gameplay.Entities;
using Bones.Gameplay.Entities.Stats;
using Bones.Gameplay.Items;
using Bones.Gameplay.Waves.Spawning.Amounts;
using Bones.Gameplay.Waves.Spawning.Positions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Bones.Gameplay.Waves
{
	[Serializable]
	//[SuffixLabel("@$value." + nameof(GetSuffix) + "()", true)]
	[InlineButton("@$value.Toggle()", "@$value.GetSuffix()")]
	public partial class SpawnerConfig
	{
		public bool Enabled;
		
		[SerializeField] private Enemy _prefab;
		[SerializeField] private DropConfig _dropConfig;
		[SerializeField] private EnemyStatsConfig _statsConfig;
		[SerializeField] private SpawnerSharedConfig _sharedConfig;
		[SerializeReference] private IAmountResolver _amountResolver = new RandomAmountResolver();
		[SerializeReference] private IPositionResolver _positionResolver = new InCirclePositionResolver();
		[SerializeField] private float _spawningDelay;
		[SerializeField] private bool _isRepeating;
		[SerializeField] private int _limit;
		[SerializeField] private PositionOriginName _origin;
		[SerializeField] private float _despawningDistance;
		
		//[Button]
		public void MigrateSharedValues()
		{
			if (!_sharedConfig) return;
			_sharedConfig.Origin = _origin;
			_sharedConfig.DespawningDistance = _despawningDistance;
		}
	}
}