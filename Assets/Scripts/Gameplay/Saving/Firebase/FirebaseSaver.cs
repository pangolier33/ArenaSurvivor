using Bones.Gameplay.Meta;
using Bones.Gameplay.Saving;
using UnityEngine;
using Bones.Gameplay.Saving.Firebase;
using System.Collections;
using Zenject;

namespace Bones.Assets.Scripts.Gameplay.Saving.Firebase
{
	public class FirebaseSaver : MonoBehaviour, ISaver
	{
		[Inject] private FirebaseClient _firebaseClient;

		public void Save(ISave save)
		{
			string json = JsonUtility.ToJson(save);
			var firabaseSave = new FirebaseSave()
			{
				fields = new FirebaseFields()
				{
					json = new FirebaseStringValue()
					{
						stringValue = json
					}
				}
			};
			string firebaseJson = JsonUtility.ToJson(firabaseSave);
			StartCoroutine(Save(firebaseJson));
		}

		private IEnumerator Save(string json)
		{
			yield return _firebaseClient.CreateSave(json);
		}
	}
}
