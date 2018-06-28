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
    public class EllipseFigure : SelectedFigureBase
    {
        private Ellipse _ellipse;
        private Vector2 _tempVector;

        public EllipseFigure(RenderTarget render) : base(render)
        {
            
        }

        public override bool SetPosition(float x, float y)
        {
            if (_tempVector == Vector2.Zero)
            {
                _tempVector.X = x;
                _tempVector.Y = y;
                return false;
            }

            var radiusX = x - _tempVector.X;
            var radiusY = y - _tempVector.Y;

            _ellipse = new Ellipse(new RawVector2(_tempVector.X + radiusX / 2, _tempVector.Y + radiusY / 2), 
                                   radiusX, radiusY);
            
            return true;
        }

        public override bool IsFigureCrossed(float x, float y)
        {
            GraphicsPath myPath = new GraphicsPath();

            myPath.AddEllipse(_ellipse.Point.X - _ellipse.RadiusX, _ellipse.Point.Y - _ellipse.RadiusY,
                _ellipse.RadiusX * 2, _ellipse.RadiusY * 2);

            bool pointWithinEllipse = myPath.IsVisible(x,y);

            return pointWithinEllipse;
        }

        public override bool ReplaceFigure(float x, float y)
        {
            throw new NotImplementedException();
        }

        public override void RenderSelectedFigure()
        {
            var ellipseCenterPoint = new Ellipse(_ellipse.Point, 5, 5);
            _render.FillEllipse(ellipseCenterPoint, _borderBrush);
            _render.DrawEllipse(ellipseCenterPoint, _borderBrush);

            var ellipseLeftTopPoint =
                new Ellipse(new RawVector2(_ellipse.Point.X + _ellipse.RadiusX, _ellipse.Point.Y), 5, 5);
            _render.FillEllipse(ellipseLeftTopPoint, _borderBrush);
            _render.DrawEllipse(ellipseLeftTopPoint, _borderBrush);

            var ellipseBottomTopPoint =
                new Ellipse(new RawVector2(_ellipse.Point.X - _ellipse.RadiusX, _ellipse.Point.Y), 5, 5);
            _render.FillEllipse(ellipseBottomTopPoint, _borderBrush);
            _render.DrawEllipse(ellipseBottomTopPoint, _borderBrush);

            var ellipseRightTopPoint =
                new Ellipse(new RawVector2(_ellipse.Point.X, _ellipse.Point.Y + _ellipse.RadiusY), 5, 5);
            _render.FillEllipse(ellipseRightTopPoint, _borderBrush);
            _render.DrawEllipse(ellipseRightTopPoint, _borderBrush);

            var ellipseRightBottomPoint =
                new Ellipse(new RawVector2(_ellipse.Point.X, _ellipse.Point.Y - _ellipse.RadiusY), 5, 5);
            _render.FillEllipse(ellipseRightBottomPoint, _borderBrush);
            _render.DrawEllipse(ellipseRightBottomPoint, _borderBrush);
        }

        public override void RenderFigure()
        { 
            _render.DrawEllipse(_ellipse, _borderBrush);
            _render.FillEllipse(_ellipse, _backgroundBrush);
        }

        public override void RenderPreview(float dx, float dy)
        {
            var ellipseMousePoint = new Ellipse(new RawVector2(dx, dy), 5, 5);
            _render.FillEllipse(ellipseMousePoint, _borderBrush);

            if (_tempVector != Vector2.Zero)
            {
                var x = dx - _tempVector.X;
                var y = dy - _tempVector.Y;
                var ellipse = new Ellipse(new RawVector2(_tempVector.X + x / 2, _tempVector.Y + y / 2), x, y);
                _render.DrawEllipse(ellipse, _borderBrush);
            }

        }
    }
}
