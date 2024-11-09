using Bones.Gameplay.Meta.Currencies;
using Bones.Gameplay.Meta;
using Bones.Gameplay.Meta.Equipment;
using Bones.Gameplay.Spells.Classes;
using UnityEngine;
using Zenject;
using Bones.Gameplay.Saving.Local;
using Bones.Gameplay.Meta.CharacterClases;
using Bones.Gameplay.Saving.UnityCloud;

namespace Bones.Gameplay.Saving
{
	public class SaveInstaller : MonoInstaller
	{
		[SerializeField]
		private EquipmentData[] _allEquipment;

		[SerializeField]
		private ClassContainer[] _allClasses;

		public override void InstallBindings()
		{
#if UNITY_WEBGL
			Container.Bind<ILoader>()
				.To<UnityCloudLoader>()
				.AsSingle();
#else
			Container.Bind<ILoader>()
				.To<LocalLoader>()
				.AsSingle();
#endif
			Container.Bind<CharacterClassUpgrader>()
				.AsSingle();

			BindMeta();
			BindSave();
		}

		private void BindMeta()
		{
			Container.Bind<ICurrencyStorage>()
				.To<CurrencyStorage>()
				.AsSingle()
				.NonLazy();

			Container.Bind<IEquipmentStorage>()
				.To<EquipmentStorage>()
				.AsSingle()
				.WithArguments(_allEquipment)
				.NonLazy();

			Container.Bind<ICharacterClassStorage>()
				.To<CharacterClassStorage>()
				.AsSingle()
				.WithArguments(_allClasses)
				.NonLazy();

			Container.Bind<Level>()
				.AsSingle()
				.NonLazy();
		}

		private void BindSave()
		{
			Container.Bind<SaveInitiator>()
			.AsSingle()
				.NonLazy();

#if UNITY_WEBGL
			Container.Bind<ISaver>()
				.To<UnityCloudSaver>()
			.AsSingle()
				.NonLazy();

			Container.Bind<ISaveCreator>()
				.To<UnityCloudSaveCreator>()
			.AsSingle()
				.NonLazy();
#else

			Container.Bind<ISaver>()
				.To<LocalSaver>()
			.AsSingle()
				.NonLazy();

			Container.Bind<ISaveCreator>()
				.To<LocalSaveCreator>()
			.AsSingle()
				.NonLazy();
#endif
		}
	}
}
