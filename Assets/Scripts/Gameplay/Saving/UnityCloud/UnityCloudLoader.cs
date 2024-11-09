using Bones.Gameplay.Meta.Currencies;
using Bones.Gameplay.Meta;
using Bones.Gameplay.Saving.Local;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Bones.Gameplay.Spells.Classes;

namespace Bones.Gameplay.Saving.UnityCloud
{
	public class UnityCloudLoader : ILoader
	{
		public const string UNITY_CLOUD_SAVE = nameof(UNITY_CLOUD_SAVE);

		public bool Loaded { get; set; }

		public async Task<ISave> Load()
		{
			await UnityServices.InitializeAsync();
			await AuthenticationService.Instance.SignInAnonymouslyAsync();
			var data = await CloudSaveService.Instance.Data.LoadAllAsync();
			Loaded = true;
			if (data.ContainsKey(UNITY_CLOUD_SAVE))
			{
				string json = data[UNITY_CLOUD_SAVE];
				return JsonUtility.FromJson<LocalSave>(json);
			}
			else
			{
				return GetFakeSave();
			}
		}

		private static ISave GetFakeSave()
		{
			return new LocalSave()
			{
				CurrencySave = new LocalCurrencySave[]
				{
					new LocalCurrencySave() { Type = CurrencyType.Silver, Value = 0},
					new LocalCurrencySave() { Type= CurrencyType.Crystal, Value = 0 }
				},
				SettingsSave = new LocalSettingsSave() 
				{ 
					CurrentLevel = 0, 
					MaxUnlockedLevel = 0 
				},
				CharacterClassStorageSave = new LocalCharacterClassStorageSave()
				{
					ClassesSave = new LocalCharacterClassSave[] { },
					ActiveClassesSave = new ClassName[0]
				},
				EquipmentStorageSave = new LocalEquipmentStorageSave()
				{
					Backpack = new LocalEquipmentSave[] { },
					Euiped = new LocalEquipmentSave[] { }
				}
			};
		}
	}
}
