using UnityEngine;

namespace Bones.UI.Views
{
	public class ExtraRareView : MonoBehaviour
	{
		[SerializeField] private Transform[] _stars;

		public void Initialize(int rareLevel)
		{
			for (int i = 0; i < rareLevel; i++)
			{
				_stars[i].gameObject.SetActive(true);
			}

			for (int i = rareLevel; i < _stars.Length; i++)
			{
				_stars[i].gameObject.SetActive(false);
			}
		}
	}
}