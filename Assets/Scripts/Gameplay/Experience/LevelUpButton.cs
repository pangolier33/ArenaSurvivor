using UnityEngine;
using Zenject;

namespace Bones.Gameplay.Experience
{
	/// <summary>
	/// added for promo by Alex
	/// </summary>
	public class LevelUpButton : MonoBehaviour
	{
		private MultiplyingExperienceBank _Expbank;

		[Inject]
		private void Inject(MultiplyingExperienceBank bankSwitcher)
		{
			_Expbank = bankSwitcher;

		}

		public void Click()
		{
			if (_Expbank != null)
				_Expbank.ObtainPercent(100);
		}
	}
}