using System;
using Bones.UI.Bindings.Base;
using UnityEngine;
using UnityEngine.UI;

namespace Bones.UI.Bindings
{
	[Serializable]
	public class ImageBinding : BaseBinding<Sprite>
	{
		[SerializeField] private Image _image;
		
		public override void OnNext(Sprite value)
		{
			_image.sprite = value;
		}
	}
}