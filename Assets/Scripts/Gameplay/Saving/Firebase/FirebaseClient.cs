using System.Collections;
using System.Text;
using UnityEngine.Networking;
using UnityEngine;

namespace Bones.Gameplay.Saving.Firebase
{
	public class FirebaseClient
	{
		private const string BASE_URL = "https://firestore.googleapis.com/v1/projects/bones-servival-web/databases/(default)/documents/saves";

		private string _token;

		public long ResponseCode { get; private set; }
		public string Message { get; private set; }
		public FirebaseSave Save { get; private set; }
		public string UserId { get; set; } = "1042456061";

		public IEnumerator CreateSave(string json)
		{
			if (string.IsNullOrWhiteSpace(UserId))
			{
				ResponseCode = 500;
				Message = "User id not set.";
				yield break;
			}

			UnityWebRequest request = new(BASE_URL + $"/{UserId}", "PATCH");

			byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
			request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
			request.SetRequestHeader("Content-Type", "application/json");

			DownloadHandler downloadHandler = new DownloadHandlerBuffer();
			request.downloadHandler = downloadHandler;
			request.timeout = 15;
			yield return request.SendWebRequest();

			if (request.responseCode != 200)
			{
				Message = request.downloadHandler.text;
				ResponseCode = request.responseCode;
				Debug.Log(ResponseCode);
				Debug.Log(Message);
			}
			else
			{
				Debug.Log("Сохранено");
			}

		}

		public IEnumerator GetSave()
		{
			if (string.IsNullOrWhiteSpace(UserId))
			{
				ResponseCode = 500;
				Message = "User id not set.";
				yield break;
			}

			UnityWebRequest request = new(BASE_URL + $"/{UserId}", "GET");
			request.SetRequestHeader("Content-Type", "application/json");
			//request.SetRequestHeader("Authorization", $"Bearer {_token}");
			DownloadHandler downloadHandler = new DownloadHandlerBuffer();
			request.downloadHandler = downloadHandler;
			request.timeout = 15;

			yield return request.SendWebRequest();
			ResponseCode = request.responseCode;
			if (request.responseCode == 200)
			{
				var response = JsonUtility.FromJson<FirebaseSave>(request.downloadHandler.text);
				Save = response;
			}
			else
			{
				Message = request.downloadHandler.text;
			}
		}
	}
}
