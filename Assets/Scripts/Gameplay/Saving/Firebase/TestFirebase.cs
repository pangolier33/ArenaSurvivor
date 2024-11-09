using Bones.Gameplay.Meta.Currencies;
using Bones.Gameplay.Saving.Local;
using System.Collections;
using UnityEngine;

namespace Bones.Gameplay.Saving.Firebase
{
	public class TestFirebase : MonoBehaviour
	{

		private void Start()
		{
			/*LocalSave save = new()
			{
				CurrencySave = new LocalCurrencySave[]
				{
					new LocalCurrencySave() { Type = CurrencyType.Silver, Value = 10},
					new LocalCurrencySave() { Type= CurrencyType.Crystal, Value = 10 }
				},
				SettingsSave = new LocalSettingsSave()
				{
					CurrentLevel = 0,
					MaxUnlockedLevel = 0
				},
				CharacterClassStorageSave = new LocalCharacterClassStorageSave()
				{
					OpenedClassesSave = new LocalCharacterClassSave[] { },
					ActiveClassesSave = new LocalCharacterClassSave[] { }
				},
				EquipmentStorageSave = new LocalEquipmentStorageSave()
				{
					Backpack = new LocalEquipmentSave[] { },
					Euiped = new LocalEquipmentSave[] { }
				}
			};
			FirebaseSave firebaseSave = new()
			{
				fields = new FirebaseFields()
				{
					json = new FirebaseStringValue()
					{
						stringValue = JsonUtility.ToJson(save)
					}
				}
			};*/

			//StartCoroutine(GetSave());
		}

		/*public IEnumerator GetSave()
		{
			FirebaseClient firebaseClient = new();
			yield return firebaseClient.GetSave(); //CreateSave("user-1", JsonUtility.ToJson(firebaseSave)
			var save = JsonUtility.FromJson<LocalSave>(firebaseClient.Save.fields.json.stringValue);
		}*/
	}
}