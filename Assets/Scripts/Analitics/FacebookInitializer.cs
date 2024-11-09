#if FACEBOOK
using Facebook.Unity;
#endif
using UnityEngine;

namespace Bones.Analitics
{
	public class FacebookInitializer : MonoBehaviour
	{
#if FACEBOOK
		private void Awake()
		{
			if (!FB.IsInitialized)
				FB.Init(InitCallback, OnHideUnity);
			else
				FB.ActivateApp();
		}

		private void InitCallback()
		{
			if (FB.IsInitialized)
			{
				FB.ActivateApp();
			}
			else
			{
				UnityEngine.Debug.Log("Failed to Initialize the Facebook SDK");
			}
		}

		private void OnHideUnity(bool isGameShown)
		{
			if (!isGameShown)
			{
				Time.timeScale = 0;
			}
			else
			{
				Time.timeScale = 1;
			}
		}
#endif
	}
}
