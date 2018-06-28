using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SharpDxTest_WF.DrawingsComponent.AdditionalModels;
using SharpDxTest_WF.DrawingsComponent.Base;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;

namespace SharpDxTest_WF.Drawings.Figures
{
    public class LineFigure : SelectedFigureBase
    {
        private CustomLine _line;
        public LineFigure(RenderTarget render) : base(render)
        {
            _line = new CustomLine();
        }

        public CustomLine Line
        {
            get => _line;
            set
            {
                if (value != null)
                    _line = value;
            } 
        }
        

        public override bool SetPosition(float x, float y)
        {
            var vector = new Vector2(x,y);

            return _line.SetVector(vector);
        }

        public override bool IsFigureCrossed(float dx, float dy)
        {
                float x0 = _line.Point1.X, y0 = _line.Point1.Y, x = _line.Point2.X, y = _line.Point2.Y;

                var part1 = (dy - y0) / (y - y0);
                var part2 = (dx - x0) / (x - x0);

                var rp1 = Math.Round(part1, 1);
                var rp2 = Math.Round(part2, 1);

                if (rp1.Equals(rp2))
                {
                    if (x0 >= dx && x <= dx && y0 <= dy && y >= dy || x0 <= dx && x >= dx && y0 >= dy && y <= dy ||
                        x0 >= dx && x <= dx && y0 >= dy && y <= dy || x0 <= dx && x >= dx && y0 <= dy && y >= dy)
                    {
                        return true;
                    }
                }

            return false;
        }

        public override bool ReplaceFigure(float x, float y)
        {
            throw new NotImplementedException();
        }

        public override void RenderSelectedFigure()
        {
            var ellipseStartPoint = new Ellipse(_line.Point1, 5, 5);
            _render.FillEllipse(ellipseStartPoint, _borderBrush);
            _render.DrawEllipse(ellipseStartPoint, _borderBrush);

            var middleVector = new Vector2((_line.Point1.X - _line.Point2.X) / 2 + _line.Point2.X,
                (_line.Point1.Y - _line.Point2.Y) / 2 + _line.Point2.Y);
            var ellipseOnCenter = new Ellipse(new Vector2(middleVector.X, middleVector.Y), 5, 5);
            _render.FillEllipse(ellipseOnCenter, _borderBrush);
            _render.DrawEllipse(ellipseOnCenter, _borderBrush);

            var ellipseFinishPoint = new Ellipse(_line.Point2, 5, 5);
            _render.FillEllipse(ellipseFinishPoint, _borderBrush);
            _render.DrawEllipse(ellipseFinishPoint, _borderBrush);
        }

        public override void RenderFigure()
        {
            _render.DrawLine(_line.Point1, _line.Point2, _borderBrush);
        }

        public override void RenderPreview(float dx, float dy)
        {
            var ellipseMousePoint = new Ellipse(new RawVector2(dx, dy), 5, 5);
            _render.FillEllipse(ellipseMousePoint, _borderBrush);
            _render.DrawEllipse(ellipseMousePoint, _borderBrush);

            if (_line.Point1 != Vector2.Zero)
            {
                _render.DrawLine(_line.Point1, new RawVector2(dx, dy), _borderBrush);

                var middleVector = new Vector2((_line.Point1.X - dx) / 2 + dx,
                    (_line.Point1.Y - dy) / 2 + dy);
                var ellipseOnCenter = new Ellipse(new Vector2(middleVector.X, middleVector.Y), 5, 5);
                _render.FillEllipse(ellipseOnCenter, _borderBrush);
                _render.DrawEllipse(ellipseOnCenter, _borderBrush);

                var ellipseStartPoint = new Ellipse(_line.Point1, 5, 5);
                _render.FillEllipse(ellipseStartPoint, _borderBrush);
                _render.DrawEllipse(ellipseStartPoint, _borderBrush);
            }
        }
    }
}
