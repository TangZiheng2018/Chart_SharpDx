using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharpDxTest_WF.DrawingsComponent.Base;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;

namespace SharpDxTest_WF.Drawings.Figures
{
    public class RectangleFigure : SelectedFigureBase
    {
        public RectangleFigure(RenderTarget render) : base(render)
        {
            
        }
        
        public override bool SetPosition(float x, float y)
        {
            var points = new Vector2(_rectangleF.Left, _rectangleF.Right);

            if (points != Vector2.Zero)
            {
                _rectangleF.Right = x;
                _rectangleF.Bottom = y;

                return true;
            }

            _rectangleF.Left = x;
            _rectangleF.Top = y;
            
            return false;
        }

        public override bool IsFigureCrossed(float x, float y)
        {
            GraphicsPath myPath = new GraphicsPath();

            var rect = new System.Drawing.RectangleF(_rectangleF.Left, _rectangleF.Top,
                _rectangleF.Right - _rectangleF.Left, _rectangleF.Bottom - _rectangleF.Top);

            myPath.AddRectangle(rect);

            bool pointWithinRectangle = myPath.IsVisible(x, y);

            return pointWithinRectangle;
        }

        public override bool ReplaceFigure(float x, float y)
        {
            throw new NotImplementedException();
        }

        public override void RenderSelectedFigure()
        {
            var ellipseLeftTopPoint = new Ellipse(new RawVector2(_rectangleF.Left,_rectangleF.Top), 5, 5);
            _render.FillEllipse(ellipseLeftTopPoint, _borderBrush);
            _render.DrawEllipse(ellipseLeftTopPoint, _borderBrush);

            var ellipseBottomTopPoint = new Ellipse(new RawVector2(_rectangleF.Left, _rectangleF.Bottom), 5, 5);
            _render.FillEllipse(ellipseBottomTopPoint, _borderBrush);
            _render.DrawEllipse(ellipseBottomTopPoint, _borderBrush);

            var ellipseRightTopPoint = new Ellipse(new RawVector2(_rectangleF.Right, _rectangleF.Top), 5, 5);
            _render.FillEllipse(ellipseRightTopPoint, _borderBrush);
            _render.DrawEllipse(ellipseRightTopPoint, _borderBrush);

            var ellipseRightBottomPoint = new Ellipse(new RawVector2(_rectangleF.Right, _rectangleF.Bottom), 5, 5);
            _render.FillEllipse(ellipseRightBottomPoint, _borderBrush);
            _render.DrawEllipse(ellipseRightBottomPoint, _borderBrush);
        }

        public override void RenderFigure()
        {
            _render.DrawRectangle(_rectangleF, _borderBrush);
            _render.FillRectangle(_rectangleF, _backgroundBrush);
        }

        public override void RenderPreview(float dx, float dy)
        {
            var ellipseMousePoint = new Ellipse(new RawVector2(dx, dy), 5, 5);
            _render.FillEllipse(ellipseMousePoint, _borderBrush);

            var points = new Vector2(_rectangleF.Left,_rectangleF.Top);

            if (points != Vector2.Zero)
            {
                _rectangleF = new RawRectangleF(_rectangleF.Left, _rectangleF.Top, dx, dy);
                _render.DrawRectangle(_rectangleF, _borderBrush);

                var ellipseFinishPoint = new Ellipse(new RawVector2(points.X, points.Y), 5, 5);
                _render.FillEllipse(ellipseFinishPoint, _borderBrush);
                _render.DrawEllipse(ellipseFinishPoint, _borderBrush);
            }
        }
    }
}
