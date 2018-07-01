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

        public RawRectangleF RectangleF { get => _rectangleF; set => _rectangleF = value; }

        public RectangleFigure(RenderTarget render) : base(render)
        {
            
        }
     

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

            //if (_rectangleF.Left == 0.0f && _rectangleF.Top == 0.0f)
            //{
            //    _rectangleF.Left = point.X;
            //    _rectangleF.Top = point.Y;

            //    if (_isPointSetted)
            //    {
            //        _isPointSetted = false;
            //        return false;
            //    }

            //    _isPointSetted = true;
            //    return true;
            //}

            //_rectangleF.Right = point.X;
            //_rectangleF.Bottom = point.Y;

            //if (_isPointSetted == true)
            //    _isPointSetted = false;

            //return false;
        }
        
        public override bool IsFigureCrossed(ScreenPoint point)
        {
            GraphicsPath myPath = new GraphicsPath();

            //var rect = FortRectangle();

            var minX = Math.Min(_rectangleF.Right, _rectangleF.Left);
            var maxX = Math.Max(_rectangleF.Right, _rectangleF.Left);
            var minY = Math.Min(_rectangleF.Bottom, _rectangleF.Top);
            var maxY = Math.Max(_rectangleF.Bottom, _rectangleF.Top);


            var rect = new System.Drawing.RectangleF(minX, minY,
                maxX - minX, maxY - minY);

            myPath.AddRectangle(rect);

            bool pointWithinRectangle = myPath.IsVisible(point.X, point.Y);

            return pointWithinRectangle;
        }
        
        public override SelectedFigureBase FigureToReplace(ScreenPoint point)
        {
            var leftTopVector = new Vector2(_rectangleF.Left, _rectangleF.Top);
            var leftBottomVector = new Vector2(_rectangleF.Left, _rectangleF.Bottom);
            var rightTopVector = new Vector2(_rectangleF.Right, _rectangleF.Top);
            var rightBottomVector = new Vector2(_rectangleF.Right, _rectangleF.Bottom);

            var firstPointCrossed = CheckCrossingEllipse(new Ellipse(leftTopVector, 5, 5), point);
            if (firstPointCrossed)
            {
                _rectangleF.Left = _rectangleF.Right;
                _rectangleF.Top = _rectangleF.Bottom;

                _rectangleF.Right = 0;
                _rectangleF.Bottom = 0;

                return this;
            }

            var secondPointCrossed = CheckCrossingEllipse(new Ellipse(rightBottomVector, 5, 5), point);
            if (secondPointCrossed)
            {
                _rectangleF.Right = 0;
                _rectangleF.Bottom = 0;

                return this;
            }

            return null;
        }

        public override void RenderSelectedFigure()
        {
            RenderElipse(new Ellipse(new RawVector2(_rectangleF.Left, _rectangleF.Top), 5, 5));

            RenderElipse(new Ellipse(new RawVector2(_rectangleF.Right, _rectangleF.Bottom), 5, 5));
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

            RenderElipse(new Ellipse(new RawVector2(dx, dy), 5, 5));

            var points = new Vector2(_rectangleF.Left, _rectangleF.Top);

            if (points != Vector2.Zero)
            {
                RectangleF = new RawRectangleF(_rectangleF.Left, _rectangleF.Top, dx, dy);
                Render.DrawRectangle(_rectangleF, BorderBrush);

                RenderElipse(new Ellipse(new RawVector2(points.X, points.Y), 5, 5));
            }
        }

        #endregion

    }
}
