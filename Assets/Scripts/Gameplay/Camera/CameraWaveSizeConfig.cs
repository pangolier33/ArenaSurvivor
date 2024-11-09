using System;
using UnityEngine;

namespace Bones.Gameplay.Camera
{
	[Serializable]
	public class CameraWaveSizeConfig
	{
		[SerializeField] private int _wave;
		[SerializeField] private float _sizeOffset;

		public int Wave => _wave;
		public float SizeOffset => _sizeOffset;

		public CameraWaveSizeConfig(int wave, float sizeOffset)
		{
			_wave = wave;
			_sizeOffset = sizeOffset;
		}
	}
}