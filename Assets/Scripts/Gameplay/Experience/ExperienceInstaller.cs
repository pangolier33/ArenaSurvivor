using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Bones.Gameplay.Experience
{
	public class ExperienceInstaller : MonoInstaller
	{
		private const string SEPARATOR = "\n";
		[SerializeField] private BankType _defaultType = BankType.MultipleStars;
		[SerializeField] private float _multiplier;
		[SerializeField] private int _starsLimit;

		[SerializeField] private TextAsset _textFile;

		[SerializeField, ListDrawerSettings(ShowIndexLabels = true, ShowPaging = true)]
		private int[] _levelsMap;

		public override void InstallBindings()
		{
			ParseTextFileIfNeed();

			var switcher = BuildBankSwitcher();

			Container.Bind<BankSwitcher>()
				.FromInstance(switcher)
				.AsSingle();

			Container.Bind<IExperienceBank>()
				.FromInstance(switcher)
				.AsSingle()
				.WhenInjectedInto<MultiplyingExperienceBank>();

			Container.BindInterfacesAndSelfTo<MultiplyingExperienceBank>()
				.AsSingle()
				.WithArguments(_multiplier);
		}

		private BankSwitcher BuildBankSwitcher()
		{
			Container.Bind<ExperienceBank>()
				.AsSingle()
				.WithArguments(_levelsMap)
				.WhenInjectedInto<ReleasingExperienceBank>();

			var multipleStarsBank = Container.Instantiate<ReleasingExperienceBank>(new object[] { _starsLimit });
			var oneStarBank = Container.Instantiate<ReleasingExperienceBank>(new object[] { 1 });
			var switcher = Container.Instantiate<BankSwitcher>(new object[]
			{
				new Dictionary<BankType, IExperienceBank>
				{
					{ BankType.MultipleStars, multipleStarsBank },
					{ BankType.OneStar, oneStarBank }
				}
			});
			switcher.Switch(_defaultType);
			return switcher;
		}

		[Button]
		private void EvaluateFromTable(string _tableParser)
		{
			_levelsMap = _tableParser.Split(SEPARATOR).Select(int.Parse).ToArray();
			_tableParser = null;
		}

		[Button]
		private void EvaluateFromCurve(AnimationCurve _curve)
		{
			if (_curve.keys.Length < 1)
				throw new InvalidOperationException("Curve must contain at least 1 key");

			var last = _curve.keys.Last();
			var maxLevel = (int)last.time;
			_levelsMap = new int[maxLevel];
			for (var i = maxLevel; i > 0; i--)
			{
				_levelsMap[i - 1] = Mathf.RoundToInt(_curve.Evaluate(i));
			}
		}

		[Button]
		private void CopyLevelAsString()
		{
			GUIUtility.systemCopyBuffer = string.Join(SEPARATOR, _levelsMap);
		}

		[Button]
		private void ParseTextFileIfNeed()
		{
			if (_textFile)
				_levelsMap = _textFile.text.Split(SEPARATOR).Select(int.Parse).ToArray();
		}
	}
}