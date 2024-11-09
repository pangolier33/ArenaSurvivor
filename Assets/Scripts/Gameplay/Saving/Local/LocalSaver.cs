using Bones.Gameplay.Meta;
using UnityEngine;

namespace Bones.Gameplay.Saving.Local
{
	public class LocalSaver : ISaver
	{
		public void Save(ISave save)
		{
			string json = JsonUtility.ToJson(save as LocalSave);
			PlayerPrefs.SetString(LocalLoader.LOCAL_SAVE, json);
		}
	}
}
