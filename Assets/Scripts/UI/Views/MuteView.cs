using UnityEngine;
using UnityEngine.UI;

namespace Bones.UI
{
	public class MuteView : MonoBehaviour
	{
		[SerializeField] private Sprite _muteOffSprite;
		[SerializeField] private Sprite _muteOnSprite;
		[SerializeField] private Image _muteImage;
		[SerializeField] private Button _muteButton;

		private void Start()
		{
			_muteButton.onClick.AddListener(SwitchMute);
			if (IsMute())
				_muteImage.sprite = _muteOnSprite;
			else
				_muteImage.sprite= _muteOffSprite;
		}

		private void SwitchMute()
		{
			if (IsMute())
			{
				UnMuteAllSound();
				_muteImage.sprite = _muteOffSprite;
			}
			else
			{
				MuteAllSound();
				_muteImage.sprite = _muteOnSprite;
			}
		}

		private void MuteAllSound()
		{
			AudioListener.volume = 0;
		}

		private void UnMuteAllSound()
		{
			AudioListener.volume = 1;
		}

		private bool IsMute()
		{
			return AudioListener.volume == 0;
		}
	}
}
