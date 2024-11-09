using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Layout
{
	public class RawImageAspectFitter : MonoBehaviour
	{
		[SerializeField] private bool m_adjustOnStart = true;

		protected RawImage m_image;
		protected float m_aspectRatio = 1.0f;
		protected float m_rectAspectRatio = 1.0f;

		private void Start()
		{
			if (m_adjustOnStart) ResetLayout();
		}

		private void SetupImage()
		{
			m_image ??= GetComponent<RawImage>();
			CalculateImageAspectRatio();
			CalculateTextureAspectRatio();
		}

		private void CalculateImageAspectRatio()
		{
			RectTransform rt = transform as RectTransform;
			m_rectAspectRatio = rt.rect.width / rt.rect.height;
		}

		private void CalculateTextureAspectRatio()
		{
			if (m_image == null)
			{
				//Debug.Log("CalculateAspectRatio: m_image is null");
				return;
			}
			if (m_image.texture == null)
			{
				//Debug.Log("CalculateAspectRatio: texture is null");
				return;
			}
			Texture2D texture = (Texture2D)m_image.texture;

			m_aspectRatio = (float)texture.width / texture.height;
			//Debug.Log("textW=" + texture.width + " h=" + texture.height + " ratio=" + m_aspectRatio);
		}

		[Button]
		public void ResetLayout()
		{
			SetupImage();

			bool fitY = m_aspectRatio < m_rectAspectRatio;

			SetAspectFitToImage(m_image, fitY, m_aspectRatio);
		}

		protected virtual void SetAspectFitToImage(RawImage _image,
		                                           bool yOverflow, float displayRatio)
		{
			if (_image == null)
			{
				return;
			}

			Rect rect = new Rect(0, 0, 1, 1);// default
			if (yOverflow)
			{
				rect.height = m_aspectRatio / m_rectAspectRatio;
				rect.y = (1 - rect.height) * 0.5f;
			}
			else
			{
				rect.width = m_rectAspectRatio / m_aspectRatio;
				rect.x = (1 - rect.width) * 0.5f;
			}
			_image.uvRect = rect;
		}
	}
}