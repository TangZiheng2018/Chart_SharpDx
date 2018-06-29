using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharpDxTest_WF.DrawingsComponent.Base;
using SharpDxTest_WF.HelperModels;
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
        

        #region Methods
        public override bool SetPosition(ScreenPoint point)
        {
            if (_tempVector == Vector2.Zero)
            {
                _tempVector.X = point.X;
                _tempVector.Y = point.Y;
                return false;
            }

            var radiusX = point.X - _tempVector.X;
            var radiusY = point.Y - _tempVector.Y;

            _ellipse = new Ellipse(new RawVector2(_tempVector.X + radiusX / 2, _tempVector.Y + radiusY / 2),
                                   radiusX, radiusY);

            return true;
        }

        public override bool IsFigureCrossed(ScreenPoint point)
        {
            GraphicsPath myPath = new GraphicsPath();

            myPath.AddEllipse(_ellipse.Point.X - _ellipse.RadiusX, _ellipse.Point.Y - _ellipse.RadiusY,
                _ellipse.RadiusX * 2, _ellipse.RadiusY * 2);

            bool pointWithinEllipse = myPath.IsVisible(point.X, point.Y);

            return pointWithinEllipse;
        }

        public override bool ReplaceFigure(ScreenPoint point)
        {
            throw new NotImplementedException();
        }

        public override void RenderSelectedFigure()
        {
            var ellipseCenterPoint = new Ellipse(_ellipse.Point, 5, 5);
            Render.FillEllipse(ellipseCenterPoint, BorderBrush);
            Render.DrawEllipse(ellipseCenterPoint, BorderBrush);

            var ellipseLeftTopPoint =
                new Ellipse(new RawVector2(_ellipse.Point.X + _ellipse.RadiusX, _ellipse.Point.Y), 5, 5);
            Render.FillEllipse(ellipseLeftTopPoint, BorderBrush);
            Render.DrawEllipse(ellipseLeftTopPoint, BorderBrush);

            var ellipseBottomTopPoint =
                new Ellipse(new RawVector2(_ellipse.Point.X - _ellipse.RadiusX, _ellipse.Point.Y), 5, 5);
            Render.FillEllipse(ellipseBottomTopPoint, BorderBrush);
            Render.DrawEllipse(ellipseBottomTopPoint, BorderBrush);

            var ellipseRightTopPoint =
                new Ellipse(new RawVector2(_ellipse.Point.X, _ellipse.Point.Y + _ellipse.RadiusY), 5, 5);
            Render.FillEllipse(ellipseRightTopPoint, BorderBrush);
            Render.DrawEllipse(ellipseRightTopPoint, BorderBrush);

            var ellipseRightBottomPoint =
                new Ellipse(new RawVector2(_ellipse.Point.X, _ellipse.Point.Y - _ellipse.RadiusY), 5, 5);
            Render.FillEllipse(ellipseRightBottomPoint, BorderBrush);
            Render.DrawEllipse(ellipseRightBottomPoint, BorderBrush);
        }

        public override void RenderPreview(ScreenPoint point)
        {
            var dx = point.X;
            var dy = point.Y;

            var ellipseMousePoint = new Ellipse(new RawVector2(dx, dy), 5, 5);
            Render.FillEllipse(ellipseMousePoint, BorderBrush);

            if (_tempVector != Vector2.Zero)
            {
                var x = dx - _tempVector.X;
                var y = dy - _tempVector.Y;
                var ellipse = new Ellipse(new RawVector2(_tempVector.X + x / 2, _tempVector.Y + y / 2), x, y);
                Render.DrawEllipse(ellipse, BorderBrush);
            }

        }

        public override void StartRendering()
        {
            Render.DrawEllipse(_ellipse, BorderBrush);
            Render.FillEllipse(_ellipse, BackgroundBrush);
        }

        #endregion

    }
}
