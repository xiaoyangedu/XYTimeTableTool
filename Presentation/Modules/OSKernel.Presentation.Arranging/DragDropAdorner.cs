using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace OSKernel.Presentation.Arranging
{
    /// <summary>
    /// 拖拽装饰器
    /// </summary>
    public class DragDropAdorner : Adorner
    {
        FrameworkElement _draggedElement = null;
        public DragDropAdorner(UIElement adornedElement) : base(adornedElement)
        {
            // Seems Adorner is hit test visible?
            IsHitTestVisible = false;

            _draggedElement = adornedElement as FrameworkElement;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            if (_draggedElement != null)
            {
                Win32.POINT screenPos = new Win32.POINT();

                if (Win32.GetCursorPos(ref screenPos))
                {
                    Point pos = PointFromScreen(new Point(screenPos.X, screenPos.Y));

                    Rect rect = new Rect(pos.X, pos.Y, _draggedElement.ActualWidth, _draggedElement.ActualHeight);
                    drawingContext.PushOpacity(1.0);

                    Brush highlight = _draggedElement.TryFindResource(SystemColors.HighlightBrushKey) as Brush;

                    if (highlight != null)
                    {
                        drawingContext.DrawRectangle(highlight, new Pen(Brushes.Transparent, 0), rect);
                    }

                    drawingContext.DrawRectangle(new VisualBrush(_draggedElement), new Pen(Brushes.Transparent, 0), rect);
                    drawingContext.Pop();
                }
            }
        }
    }
}
