using System.Drawing.Drawing2D;
using SharpDxTest_WF.HelperModels;
using SharpDX.Direct2D1;

namespace SharpDxTest_WF.DrawingsComponent.Base
{
    public abstract class SelectedFigureBase : DrawingBase
    {
        protected SelectedFigureBase(RenderTarget render) : base(render)
        {

        }

        public abstract bool SetPosition(ScreenPoint point);

        public abstract bool IsFigureCrossed(ScreenPoint point);
        
        public abstract void RenderSelectedFigure();

        public abstract SelectedFigureBase FigureToReplace(ScreenPoint point);

        protected void RenderElipse(Ellipse ellipse)
        {
            Render.FillEllipse(ellipse, BackgroundBrush);
            Render.DrawEllipse(ellipse, BorderBrush);
        }

        public bool CheckCrossingEllipse(Ellipse ellipse, ScreenPoint point)
        {
            GraphicsPath myPath = new GraphicsPath();

            myPath.AddEllipse(ellipse.Point.X - ellipse.RadiusX, ellipse.Point.Y - ellipse.RadiusY,
                ellipse.RadiusX * 2, ellipse.RadiusY * 2);

            return myPath.IsVisible(point.X, point.Y);
        }

    }
}
