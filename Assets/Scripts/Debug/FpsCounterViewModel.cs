using TMPro;
using UnityEngine;

namespace Bones.Debug
{
	public class FpsCounterViewModel : MonoBehaviour
	{
		[SerializeField] private float _updateInterval;
		[SerializeField] private TMP_Text _fpsLabel;
		[SerializeField] private string _format = "FPS:{0}";

		private int _lastFramesCount;
		private float _timestamp;

		private void LateUpdate()
		{
			var time = Time.time;

			if (time < _timestamp)
				return;

			var currentFrameCount = Time.frameCount;
			var deltaFrames = (currentFrameCount - _lastFramesCount) / _updateInterval;
			UpdateFps(deltaFrames);

			_lastFramesCount = currentFrameCount;
			_timestamp = time + _updateInterval;
		}

		private void UpdateFps(float fps) => _fpsLabel.text = string.Format(_format, (int)fps);
	}
}