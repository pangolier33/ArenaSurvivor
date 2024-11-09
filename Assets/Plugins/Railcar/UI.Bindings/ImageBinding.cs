using System;
using UnityEngine;
using UnityEngine.UI;

namespace Railcar.UI.Bindings
{
	[Serializable]
	internal sealed class ImageBinding : BaseBinding<Sprite>
	{
		[SerializeField] private Image _image;
		
		public override void OnNext(Sprite value) => _image.sprite = value;
	}
}