/// Skewed Grid Layout Group
/// ===============================
/// 
/// Written by Martin Nerurkar
/// - http://www.martin.nerurkar.de
/// - http://www.sharkbombs.com
///
/// You are free to moddify and use this code in your own projects 
/// You are not allowed to remove this copyright notice
/// You are not allowed to resell this code, even if modified

namespace UnityEngine.UI
{
	[AddComponentMenu("Layout/Skewed Grid Layout Group", 152)]
	public class SkewedGridLayoutGroup : GridLayoutGroup
	{
		[SerializeField]
		protected Axis m_skewAxis = Axis.Horizontal;
		public Axis skewAxis { get { return m_skewAxis; } set { SetProperty(ref m_skewAxis, value); } }
		
		[SerializeField]
		protected bool m_useMaxSkewDistance = false;
		public bool useMaxSkewDistance { get { return m_useMaxSkewDistance; } set { SetProperty(ref m_useMaxSkewDistance, value); } }
		
		[SerializeField]
		protected Vector2 m_maxSkewDistance = Vector2.zero;
		public Vector2 maxSkewDistance { get { return m_maxSkewDistance; } set { SetProperty(ref m_maxSkewDistance, value); } }
		
		protected SkewedGridLayoutGroup() { }
		
		public override void SetLayoutHorizontal()
		{
			SetCellsAlongAxis(0);
		}
		
		public override void SetLayoutVertical()
		{
			SetCellsAlongAxis(1);
		}
		
		// Based on SetCellsAlongAxis from GridLayoutGroup with appropriate changes
		private void SetCellsAlongAxis(int axis)
		{
			// Normally a Layout Controller should only set horizontal values when invoked for the horizontal axis
			// and only vertical values when invoked for the vertical axis.
			// However, in this case we set both the horizontal and vertical position when invoked for the vertical axis.
			// Since we only set the horizontal position and not the size, it shouldn't affect children's layout,
			// and thus shouldn't break the rule that all horizontal layout must be calculated before all vertical layout.
			
			if (axis == 0)
			{
				// Only set the sizes when invoked for horizontal axis, not the positions.
				for (int i = 0; i < rectChildren.Count; i++)
				{
					RectTransform rect = rectChildren[i];
					
					m_Tracker.Add(this, rect,
					              DrivenTransformProperties.Anchors |
					              DrivenTransformProperties.AnchoredPosition |
					              DrivenTransformProperties.SizeDelta);
					
					rect.anchorMin = Vector2.up;
					rect.anchorMax = Vector2.up;
					rect.sizeDelta = cellSize;
				}
				return;
			}
			
			float width = rectTransform.rect.size.x;
			float height = rectTransform.rect.size.y;
			float paddedWidth = width - padding.horizontal;
			float paddedHeight = height - padding.vertical;
			
			int cellCountX = 1;
			int cellCountY = 1;
			
			if (m_Constraint == Constraint.FixedColumnCount)
			{
				cellCountX = m_ConstraintCount;
				cellCountY = Mathf.CeilToInt(rectChildren.Count / (float)cellCountX - 0.001f);
			}
			else if (m_Constraint == Constraint.FixedRowCount)
			{
				cellCountY = m_ConstraintCount;
				cellCountX = Mathf.CeilToInt(rectChildren.Count / (float)cellCountY - 0.001f);
			}
			else
			{
				if (cellSize.x + spacing.x <= 0)
					cellCountX = int.MaxValue;
				else
					cellCountX = Mathf.Max(1, Mathf.FloorToInt((paddedWidth + spacing.x + 0.001f) / (cellSize.x + spacing.x)));
				
				if (cellSize.y + spacing.y <= 0)
					cellCountY = int.MaxValue;
				else
					cellCountY = Mathf.Max(1, Mathf.FloorToInt((paddedHeight + spacing.y + 0.001f) / (cellSize.y + spacing.y)));
			}
			
			int cornerX = (int)startCorner % 2;
			int cornerY = (int)startCorner / 2;
			
			int cellsPerMainAxis, actualCellCountX, actualCellCountY;
			if (startAxis == Axis.Horizontal)
			{
				cellsPerMainAxis = cellCountX;
				actualCellCountX = Mathf.Clamp(cellCountX, 1, rectChildren.Count);
				actualCellCountY = Mathf.Clamp(cellCountY, 1, Mathf.CeilToInt(rectChildren.Count / (float)cellsPerMainAxis));
			}
			else
			{
				cellsPerMainAxis = cellCountY;
				actualCellCountY = Mathf.Clamp(cellCountY, 1, rectChildren.Count);
				actualCellCountX = Mathf.Clamp(cellCountX, 1, Mathf.CeilToInt(rectChildren.Count / (float)cellsPerMainAxis));
			}
			
			Vector2 requiredSpace = new Vector2(
				actualCellCountX * cellSize.x + (actualCellCountX - 1) * spacing.x,
				actualCellCountY * cellSize.y + (actualCellCountY - 1) * spacing.y
				);
			
			Vector2 remainingSpacePerAxisStep = new Vector2(
				(paddedWidth - requiredSpace.x) / Mathf.Max(1, actualCellCountY - 1),
				(paddedHeight - requiredSpace.y) / Mathf.Max(1, actualCellCountX - 1)
				);
			
			if (useMaxSkewDistance)
			{
				remainingSpacePerAxisStep = new Vector2(
					Mathf.Min(maxSkewDistance.x, remainingSpacePerAxisStep.x),
					Mathf.Min(maxSkewDistance.y, remainingSpacePerAxisStep.y)
					);
			}
			
			Vector2 usedSpace = new Vector2(
				(skewAxis == Axis.Horizontal ? (requiredSpace.x + remainingSpacePerAxisStep.x * (actualCellCountY - 1)) : requiredSpace.x),
				(skewAxis == Axis.Vertical ? (requiredSpace.y + remainingSpacePerAxisStep.y * (actualCellCountX - 1)) : requiredSpace.y)
				);
			
			Vector2 startOffset = new Vector2(
				GetStartOffset(0, usedSpace.x),
				GetStartOffset(1, usedSpace.y)
				);
			
			int positionX;
			int positionY;
			float skewX;
			float skewY;
			
			for (int i = 0; i < rectChildren.Count; i++)
			{
				if (startAxis == Axis.Horizontal)
				{
					positionX = i % cellsPerMainAxis;
					positionY = i / cellsPerMainAxis;
				}
				else
				{
					positionX = i / cellsPerMainAxis;
					positionY = i % cellsPerMainAxis;
				}
				
				if (cornerX == 1)
					positionX = actualCellCountX - 1 - positionX;
				if (cornerY == 1)
					positionY = actualCellCountY - 1 - positionY;
				
				skewX = (skewAxis == Axis.Horizontal ? remainingSpacePerAxisStep.x * positionY : 0f);
				skewY = (skewAxis == Axis.Vertical ? remainingSpacePerAxisStep.y * positionX : 0f);
				
				SetChildAlongAxis(rectChildren[i], 0, startOffset.x + (cellSize[0] + spacing[0]) * positionX + skewX, cellSize[0]);
				SetChildAlongAxis(rectChildren[i], 1, startOffset.y + (cellSize[1] + spacing[1]) * positionY + skewY, cellSize[1]);
			}
		}
	}
}