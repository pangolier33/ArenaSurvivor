using Sirenix.OdinInspector;
using UnityEngine;

namespace Mitaywalle.ProceduralUI
{
    public class Widget : MonoBehaviour
    {
        public RectTransform rectTransform => transform as RectTransform;

        [Button]
        public void StretchToParent(bool pivotToCenter = false)
        {
            RectTransform tr = rectTransform;
            if (pivotToCenter) tr.pivot = new Vector2(.5f, .5f);
            tr.localScale = Vector3.one;
            tr.localPosition = Vector3.zero;
            tr.localRotation = Quaternion.identity;
            tr.anchoredPosition = Vector2.zero;
            tr.anchorMin = Vector2.zero;
            tr.anchorMax = Vector2.one;
            tr.sizeDelta = Vector2.zero;
        }
    }
}
