using System.Drawing.Drawing2D;
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

        public override void RenderSelectedFigure()
        {
            RenderElipse(new Ellipse(_ellipse.Point, 5, 5));

            RenderElipse(new Ellipse(new RawVector2(_ellipse.Point.X, _ellipse.Point.Y + _ellipse.RadiusY), 5, 5));

            RenderElipse(new Ellipse(new RawVector2(_ellipse.Point.X, _ellipse.Point.Y - _ellipse.RadiusY), 5, 5));
        }

        public override SelectedFigureBase FigureToReplace(ScreenPoint point)
        {
            //var firstPoint = CheckCrossingEllipse(new Ellipse(new RawVector2(_ellipse.Point.X, _ellipse.Point.Y + _ellipse.RadiusY), 5, 5), point);
            //if (firstPoint)
            //{
            //    _tempVector.X = point.X;
            //    _tempVector.Y = point.Y;


            //}

            //var secondPoint = CheckCrossingEllipse(new Ellipse(new RawVector2(_ellipse.Point.X, _ellipse.Point.Y - _ellipse.RadiusY), 5, 5), point);
            //if (secondPoint)
            //{
            //    _tempVector.X = point.X;
            //    _tempVector.Y = point.Y;


            //}
            return this;
        }

        public override void RenderPreview(ScreenPoint point)
        {
            var dx = point.X;
            var dy = point.Y;

            RenderElipse(new Ellipse(new RawVector2(dx, dy), 5, 5));

            if (_tempVector != Vector2.Zero)
            {
                var x = dx - _tempVector.X;
                var y = dy - _tempVector.Y;
                RenderElipse(new Ellipse(new RawVector2(_tempVector.X + x / 2, _tempVector.Y + y / 2), x, y));
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
