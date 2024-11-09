using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Mitaywalle.ProceduralUI
{
	public class SwitchGo : MonoBehaviour
	{
		[SerializeField] bool showRandomOnEnable = false;
		public List<GameObject> Targets = new();

		void OnEnable()
		{
			if (showRandomOnEnable) ShowRandom();
		}

		public void HideAll()
		{
			for (int i = 0; i < Targets.Count; i++)
			{
				Targets[i].gameObject.SetActive(false);
			}
		}
		[Button]
		public void ShowRandom()
		{
			int index = Random.Range(0, Targets.Count);
			for (int i = 0; i < Targets.Count; i++)
			{
				Targets[i].gameObject.SetActive(i == index);
			}
		}
		public void ShowOnly(int index)
		{
			for (int i = 0; i < Targets.Count; i++)
			{
				Targets[i].gameObject.SetActive(i == index);
			}
		}

		public void ShowAll()
		{
			for (int i = 0; i < Targets.Count; i++)
			{
				Targets[i].gameObject.SetActive(true);
			}
		}
	}
}