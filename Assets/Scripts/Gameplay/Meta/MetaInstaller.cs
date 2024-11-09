using Bones.Gameplay.Meta;
using Bones.Gameplay.Meta.CharacterClases;
using Bones.Gameplay.Meta.Equipment;
using UnityEngine;
using Zenject;

namespace Bones.Gameplay.Experience
{
	public class MetaInstaller : MonoInstaller
	{
		[SerializeField] private IntDataMap _experienceValueToClassLevelUpMap;
		[SerializeField] private IntDataMap _softValueToClassLevelUpMap;
		[SerializeField] private FloatDataMap _damageMultiplierClassLevelMap;
		[SerializeField] private IntDataMap _copiesClassToUpgradeRareMap;
		[SerializeField] private IntDataMap _softValueToUpgradeRareMap;

		[SerializeField] private FloatDataMap _equipmentLevelMultiplierMap;
		[SerializeField] private FloatDataMap _equipmentRareMultiplierMap;
		[SerializeField] private IntDataMap _softToMergeMap;
		[SerializeField] private IntDataMap _softToSlotUpgradeMap;
		[SerializeField] private IntDataMap _recipesToSlotUpgradeMap;

		[SerializeField] private RareColor[] _rareColors;

		public override void InstallBindings()
		{
			ExperienceToLevelUpCalculator.Initialize(_experienceValueToClassLevelUpMap.DataMap);
			SoftToLevelUpCalculator.Initialize(_softValueToClassLevelUpMap.DataMap);
			ClassLevelMuliplierCalculator.Initialize(_damageMultiplierClassLevelMap.DataMap);
			SoftToUpgradeRareCalculator.Initialize(_softValueToUpgradeRareMap.DataMap);
			CopiesToUpgradeRareCalculator.Initialize(_copiesClassToUpgradeRareMap.DataMap);

			EquipmentLevelMultiplierCalculator.Initialize(_equipmentLevelMultiplierMap.DataMap);
			EquipmentRareMultiplierCalculator.Initialize(_equipmentRareMultiplierMap.DataMap);
			SoftToMergeCalculator.Initialize(_softToMergeMap.DataMap);
			SoftToSlotUpgradeCalculator.Initialize(_softToSlotUpgradeMap.DataMap);
			RecipesToSlotUpgradeCalculator.Initialize(_recipesToSlotUpgradeMap.DataMap);

			RareColorDeterminator.Initialize(_rareColors);
		}
	}
}