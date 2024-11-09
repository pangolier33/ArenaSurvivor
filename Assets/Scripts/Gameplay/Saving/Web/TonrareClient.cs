using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Bones.Web
{
	public class TonrareClient
	{
		private const string BASE_URL = "https://tg-game-bot.tonrare.com/";

		private string _token;
		
		public long ResponseCode { get; private set; }
		public TonrareUser User { get; private set; }
		public string Message { get; private set; }
		public bool Authorized { get; set; }

		public IEnumerator Authorize()
		{
			string[] queryParams = Application.absoluteURL.Split('#');
			if (queryParams.Length != 2 )
			{
				ResponseCode = 500;
				Message = "Don't hava telegram data";
				yield break;
			}
			UnityWebRequest request = new(BASE_URL + "game-api/auth/telegram-webapp", "POST");
			AuthorizeRequestData data = new()
			{
				auth_data = queryParams[1],
				bot_id = 1
			};
			string json = JsonUtility.ToJson(data);
			byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
			request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
			request.SetRequestHeader("Content-Type", "application/json");

			DownloadHandler downloadHandler = new DownloadHandlerBuffer();
			request.downloadHandler = downloadHandler;
			request.timeout = 15;
			yield return request.SendWebRequest();
			ResponseCode = request.responseCode;
			if (request.responseCode == 200)
			{
				var response = JsonUtility.FromJson<AuthorizeResponseData>(request.downloadHandler.text);
				_token = response.token;
			}
			else
			{
				Message = request.downloadHandler.text;
			}
		}

		public IEnumerator GetUser()
		{
			UnityWebRequest request = new(BASE_URL + "game-api/auth/user", "GET");
			request.SetRequestHeader("Content-Type", "application/json");
			request.SetRequestHeader("Authorization", $"Bearer {_token}");
			DownloadHandler downloadHandler = new DownloadHandlerBuffer();
			request.downloadHandler = downloadHandler;
			request.timeout = 15;

			yield return request.SendWebRequest();
			ResponseCode = request.responseCode;
			if (request.responseCode == 200)
			{
				var response = JsonUtility.FromJson<UserResponseData>(request.downloadHandler.text);
				User = response.user;
				Authorized = true;
			}
			else
			{
				Message = request.downloadHandler.text;
			}
		}
	}
}
