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
    public class RectangleFigure : SelectedFigureBase
    {
        private RawRectangleF _rectangleF;

        public RectangleFigure(RenderTarget render) : base(render)
        {
            
        }
     
        protected RectangleF Rectangle { get;set; }

        #region Methods
        
        public override bool SetPosition(ScreenPoint point)
        {
            var points = new Vector2(_rectangleF.Left, _rectangleF.Right);

            if (points != Vector2.Zero)
            {
                _rectangleF.Right = point.X;
                _rectangleF.Bottom = point.Y;

                return true;
            }

            _rectangleF.Left = point.X;
            _rectangleF.Top = point.Y;

            return false;
        }

        public override bool IsFigureCrossed(ScreenPoint point)
        {
            GraphicsPath myPath = new GraphicsPath();

            var rect = new System.Drawing.RectangleF(_rectangleF.Left, _rectangleF.Top,
                _rectangleF.Right - _rectangleF.Left, _rectangleF.Bottom - _rectangleF.Top);

            myPath.AddRectangle(rect);

            bool pointWithinRectangle = myPath.IsVisible(point.X, point.Y);

            return pointWithinRectangle;
        }

        public override bool ReplaceFigure(ScreenPoint point)
        {
            throw new NotImplementedException();
        }

        public override void RenderSelectedFigure()
        {
            var ellipseLeftTopPoint = new Ellipse(new RawVector2(_rectangleF.Left, _rectangleF.Top), 5, 5);
            Render.FillEllipse(ellipseLeftTopPoint, BorderBrush);
            Render.DrawEllipse(ellipseLeftTopPoint, BorderBrush);

            var ellipseBottomTopPoint = new Ellipse(new RawVector2(_rectangleF.Left, _rectangleF.Bottom), 5, 5);
            Render.FillEllipse(ellipseBottomTopPoint, BorderBrush);
            Render.DrawEllipse(ellipseBottomTopPoint, BorderBrush);

            var ellipseRightTopPoint = new Ellipse(new RawVector2(_rectangleF.Right, _rectangleF.Top), 5, 5);
            Render.FillEllipse(ellipseRightTopPoint, BorderBrush);
            Render.DrawEllipse(ellipseRightTopPoint, BorderBrush);

            var ellipseRightBottomPoint = new Ellipse(new RawVector2(_rectangleF.Right, _rectangleF.Bottom), 5, 5);
            Render.FillEllipse(ellipseRightBottomPoint, BorderBrush);
            Render.DrawEllipse(ellipseRightBottomPoint, BorderBrush);
        }

        public override void StartRendering()
        {
            Render.DrawRectangle(_rectangleF, BorderBrush);
            Render.FillRectangle(_rectangleF, BackgroundBrush);
        }

        public override void RenderPreview(ScreenPoint point)
        {
            var dx = point.X;
            var dy = point.Y;

            var ellipseMousePoint = new Ellipse(new RawVector2(dx, dy), 5, 5);
            Render.FillEllipse(ellipseMousePoint, BorderBrush);

            var points = new Vector2(_rectangleF.Left, _rectangleF.Top);

            if (points != Vector2.Zero)
            {
                _rectangleF = new RawRectangleF(_rectangleF.Left, _rectangleF.Top, dx, dy);
                Render.DrawRectangle(_rectangleF, BorderBrush);

                var ellipseFinishPoint = new Ellipse(new RawVector2(points.X, points.Y), 5, 5);
                Render.FillEllipse(ellipseFinishPoint, BorderBrush);
                Render.DrawEllipse(ellipseFinishPoint, BorderBrush);
            }
        }

        #endregion

    }
}
