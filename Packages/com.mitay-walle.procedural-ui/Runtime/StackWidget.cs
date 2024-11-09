using UnityEngine;
using UnityEngine.UI;

namespace Mitaywalle.ProceduralUI
{
    public class StackWidget : Widget
    {
        public WidgetsFactory Widgets;
        public WidgetsLayout Layout;

        private void OnEnable()
        {
            ResetLayout();
        }

        public void ResetLayout()
        {
            LayoutRebuilder.MarkLayoutForRebuild(transform as RectTransform);
        }
    }
}
