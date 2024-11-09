using Bones.Gameplay.Meta;
using System.Collections.Generic;
using Unity.Services.CloudSave;
using UnityEngine;

namespace Bones.Gameplay.Saving.UnityCloud
{
	public class UnityCloudSaver : ISaver
	{
		public async void Save(ISave save)
		{
			string json = JsonUtility.ToJson(save);
			var data = new Dictionary<string, object> { { UnityCloudLoader.UNITY_CLOUD_SAVE, json } };
			await CloudSaveService.Instance.Data.ForceSaveAsync(data);
		}
	}
}
