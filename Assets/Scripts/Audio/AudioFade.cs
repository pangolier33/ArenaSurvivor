using System.Collections;
using UnityEngine;

public class AudioFade : MonoBehaviour
{
	[SerializeField] private AudioSource _source;
	[SerializeField] private float _startVolume;
	[SerializeField] private float _endVolume;
	[SerializeField] private float _duration;

	private void Start()
	{
		StartCoroutine(FadeOut());
	}

	private IEnumerator FadeOut()
	{
		_source.volume = _startVolume;
		while (_source.volume < _endVolume)
		{
			_source.volume += Time.deltaTime / _duration;

			yield return null;
		}
		_source.volume = _endVolume;
	}
}
