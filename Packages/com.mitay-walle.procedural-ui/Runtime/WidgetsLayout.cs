using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Mitaywalle.ProceduralUI
{
    [Serializable]
    public class WidgetsLayout
    {
        [SerializeField] private RectTransform _transform;

        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private LayoutGroup _layoutGroup;
        [SerializeField] private ContentSizeFitter _fitter;

        [Button]
        public void MakeGrid(int size, int sizeY = 0)
        {
            if (sizeY <= 0) sizeY = size;

            var group = RecreateLayoutGroup<GridLayoutGroup>();
            group.cellSize = new Vector2(size, sizeY);
        }

        [Button]
        public void MakeVertical(bool fitChild = true, bool expandOtherAxis = true)
        {
            var group = RecreateLayoutGroup<VerticalLayoutGroup>();

            group.childControlWidth = expandOtherAxis;
            group.childForceExpandWidth = expandOtherAxis;

            group.childControlHeight = !fitChild;
            group.childForceExpandHeight = !fitChild;

            _fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            _fitter.verticalFit = fitChild ? ContentSizeFitter.FitMode.PreferredSize : ContentSizeFitter.FitMode.Unconstrained;
        }

        [Button]
        public void MakeHorizontal(bool fitChild = true, bool expandOtherAxis = true)
        {
            var group = RecreateLayoutGroup<HorizontalLayoutGroup>();

            group.childControlWidth = !fitChild;
            group.childForceExpandWidth = !fitChild;

            group.childControlHeight = expandOtherAxis;
            group.childForceExpandHeight = expandOtherAxis;

            _fitter.horizontalFit = fitChild ? ContentSizeFitter.FitMode.PreferredSize : ContentSizeFitter.FitMode.Unconstrained;
            _fitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
        }

        [Button]
        public void MakeScrollRect()
        {
            if (_scrollRect) return;

            var ScrollRectTr = new GameObject($"{_transform.name} - ScrollRect").AddComponent<RectMask2D>().rectTransform;
            _scrollRect = ScrollRectTr.gameObject.AddComponent<ScrollRect>();
            _scrollRect.content = _transform;
            _transform.name += " - Content";
            ScrollRectTr.SetParent(_transform.parent);
            StretchToParent(ScrollRectTr);
            _transform.SetParent(ScrollRectTr);
            StretchToParent(_transform);
            _transform.gameObject.SetActive(true);


            switch (_layoutGroup)
            {
                case HorizontalLayoutGroup group:
                {
                    _scrollRect.vertical = false;

                    break;
                }

                case VerticalLayoutGroup group:
                {
                    _scrollRect.horizontal = false;

                    break;
                }

                case GridLayoutGroup group:
                {
                    //_scrollRect.horizontal = false;
                    break;
                }
            }
        }

        private T RecreateLayoutGroup<T>()
            where T : LayoutGroup
        {
            if (_layoutGroup == null)
            {
                _layoutGroup = _transform.gameObject.AddComponent<T>();
            }
            else
            {
                if (_layoutGroup is not T)
                {
                    DestroySafe(_layoutGroup);
                    _layoutGroup = _transform.gameObject.AddComponent<T>();
                }
            }

            return _layoutGroup as T;
        }

        private void DestroySafe(Object target)
        {
            if (target == null) return;

            if (Application.isPlaying)
            {
                Object.DestroyImmediate(target);
            }
            else
            {
                Object.DestroyImmediate(target);
            }
        }
        public void SetSpacing(int value)
        {
            switch (_layoutGroup)
            {
                case HorizontalOrVerticalLayoutGroup group:
                {
                    group.spacing = value;

                    break;
                }

                case GridLayoutGroup group:
                {
                    group.spacing = Vector2.one * value;

                    break;
                }
            }
        }

        private void StretchToParent(RectTransform rectTransform, bool pivotToCenter = false)
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
