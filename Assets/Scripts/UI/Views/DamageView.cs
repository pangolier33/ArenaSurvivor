using Bones.Gameplay.Entities;
using Bones.Gameplay.Events.Args;
using System;
using TMPro;
using UnityEngine;

namespace UI.Views
{
	public class DamageView : Damage
	{
		[SerializeField] private TMP_Text _damage;
		[SerializeField] private Color _normalColor;
		[SerializeField] private Color _bigDamageColor;
		[SerializeField] private float _bigDamageValue;
		[SerializeField] private UnityEngine.Animation _animation;
		[SerializeField] private float _lifeTime;

		public override float LifeTime => _lifeTime;

		public override void SetValue(EnemyDamagedArgs enemyDamagedArgs)
		{
			_damage.SetText(GetFormatedValue(enemyDamagedArgs.Value));

			if (enemyDamagedArgs.RelativeValue >= _bigDamageValue)
				_damage.color = _bigDamageColor;
			else
				_damage.color = _normalColor;
		}

		private string GetFormatedValue(double value)
		{
			int floorValue = (int)value;
			if (floorValue < 1000)
				return floorValue.ToString();
			else
				return $"{floorValue / 1000}K";
		}

		public override void Start()
		{
			_animation.Play();
		}
	}
}
