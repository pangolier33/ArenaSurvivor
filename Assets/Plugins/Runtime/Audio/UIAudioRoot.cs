using UnityEngine;

namespace Plugins.Audio
{
	public class UIAudioRoot : MonoBehaviour
	{
		[SerializeField] private AudioClip _onPointerEnter;
		[SerializeField] private AudioClip _onClick;
		[SerializeField] private AudioClip _onMove;
		[SerializeField] private AudioSource _source;
		[SerializeField] private AudioClip _onWindowAppear;
		
		public void OnWindowAppear()
		{
			if (_onWindowAppear)
			{
				if (_source)
				{
					_source.PlayOneShot(_onWindowAppear);
				}
				else
				{
					AudioSource.PlayClipAtPoint(_onWindowAppear, Vector3.zero);
				}
			}
		}
		public void OnPointerEnter()
		{
			if (_onPointerEnter)
			{
				if (_source)
				{
					_source.PlayOneShot(_onPointerEnter);
				}
				else
				{
					AudioSource.PlayClipAtPoint(_onPointerEnter, Vector3.zero);
				}
			}
		}
		public void OnClick()
		{
			if (_onClick)
			{
				if (_source)
				{
					_source.PlayOneShot(_onClick);
				}
				else
				{
					AudioSource.PlayClipAtPoint(_onClick, Vector3.zero);
				}
			}
		}
		public void OnMove()
		{
			if (_onMove)
			{
				if (_source)
				{
					_source.PlayOneShot(_onMove);
				}
				else
				{
					AudioSource.PlayClipAtPoint(_onMove, Vector3.zero);
				}
			}
		}
	}
}