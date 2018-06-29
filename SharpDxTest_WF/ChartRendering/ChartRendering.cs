using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using SharpDxTest_WF.BarComponent.Models;
using SharpDxTest_WF.Models;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;
using Text = SharpDX.DirectWrite.TextFormat;
using SharpDX.DirectWrite;

namespace SharpDxTest_WF.ChartRendering
{
    public class ChartRendering : RenderingBase
    {
        private float _textSize = 0;
        private string _textValue = "";
        private Text _textFormat;
        private ChartDrawing _chartDrawing;


        public ChartDrawing ChartDrawing
        {
            get => _chartDrawing;
            set => _chartDrawing = value ?? throw new NullReferenceException();
        }

        public Text TextFormat
        {
            get => _textFormat;
            set => _textFormat = value ?? throw new NullReferenceException();
        }


        public ChartRendering(RenderTarget renderTarget, Text textFormat, ChartDrawing chartDrawing) : base(renderTarget)
        {
            ChartDrawing = chartDrawing;
            TextFormat = textFormat;
            _textSize = TextFormat.FontSize;
        }


        public override void StartRendering()
        {
            RenderAxeX();
            RenderAxeY();
        }

        private void RenderAxeY()
        {
            var yVectors = _chartDrawing.YAxeVectors();
            Render.DrawLine(yVectors[0], yVectors[1], _chartDrawing.Brushes.Black, 2);

            //set Y axe and point on it
            for (int i = 1; i <= _chartDrawing.AxeSetting.CountPointsOnYAxe; i++)
            {
                var vectorPoint1 = new Vector2(_chartDrawing.AxeSetting.YValuePointStart,
                    _chartDrawing.AxeSetting.TouchMiddlePoint.Y - _chartDrawing.ChartHeight * _chartDrawing.AxeSetting.PointOnEveryPercentAxeY * i);

                var vectorPoint2 = new Vector2(_chartDrawing.AxeSetting.YValuePointFinish,
                    _chartDrawing.AxeSetting.TouchMiddlePoint.Y - _chartDrawing.ChartHeight * _chartDrawing.AxeSetting.PointOnEveryPercentAxeY * i);

                var diffBetweenValues = (_chartDrawing.MinMaxValues.MaxValueLocation - _chartDrawing.MinMaxValues.MinValueLocation) /
                                        _chartDrawing.AxeSetting.CountPointsOnYAxe;

                _textValue = Convert.ToString(Math.Round(_chartDrawing.MinMaxValues.MinValueLocation + diffBetweenValues * i, 2));

                var textPlacement = new RawRectangleF(_chartDrawing.WindowWidth * (_chartDrawing.Paddings.PaddingRightRatio + 0.025f),
                    (_chartDrawing.AxeSetting.TouchMiddlePoint.Y - _chartDrawing.ChartHeight * _chartDrawing.AxeSetting.PointOnEveryPercentAxeY * i)
                    - _textSize / 2 - 0.25f * _textSize,
                    Render.Size.Width, Render.Size.Height);

                Render.DrawText(_textValue, TextFormat, textPlacement, _chartDrawing.Brushes.Black);
                Render.DrawLine(vectorPoint1, vectorPoint2, _chartDrawing.Brushes.Black, 2);
            }
        }

        private void RenderAxeX()
        {
            var xVectors = _chartDrawing.XAxeVectors();
            Render.DrawLine(xVectors[0], xVectors[1], _chartDrawing.Brushes.Black, 2);

            //set X axe and point on it
            for (int i = 0; i < _chartDrawing.AxeSetting.CountPointsOnXAxe; i++)
            {
                var leftPadding = _chartDrawing.AxeSetting.TouchMiddlePoint.X -
                                  _chartDrawing.ChartWidth * _chartDrawing.AxeSetting.PointOnEveryPercentAxeX * (i + 1);

                var vectorPoint1 = new Vector2(leftPadding, _chartDrawing.AxeSetting.XDatePointStart);

                var vectorPoint2 = new Vector2(leftPadding, _chartDrawing.AxeSetting.XDatePointFinish);

                var minDate = _chartDrawing.MinMaxValues.MinDateLocation;
                var maxDate = _chartDrawing.MinMaxValues.MaxDateLocation;

                var pointCounts = _chartDrawing.AxeSetting.CountPointsOnXAxe;

                if (_chartDrawing.DateIn == TimingBy.Minute)
                {
                    var timePerPoint = Math.Ceiling(maxDate.Subtract(minDate).TotalMinutes / pointCounts);
                    var timeForSet = minDate.AddMinutes(timePerPoint * (_chartDrawing.AxeSetting.CountPointsOnXAxe - i));
                    _textValue = timeForSet.ToShortTimeString();
                }

                if (_chartDrawing.DateIn == TimingBy.Hour)
                {
                    var timePerPoint = Math.Ceiling(maxDate.Subtract(minDate).TotalHours / pointCounts);
                    var timeForSet = minDate.AddHours(timePerPoint * (_chartDrawing.AxeSetting.CountPointsOnXAxe - i));
                    _textValue = timeForSet.ToShortDateString() + " " + timeForSet.ToShortTimeString();
                }

                if (_chartDrawing.DateIn == TimingBy.Day)
                {
                    var timePerPoint = Math.Ceiling(maxDate.Subtract(minDate).TotalDays / pointCounts);
                    var timeForSet = minDate.AddDays(timePerPoint * (_chartDrawing.AxeSetting.CountPointsOnXAxe - i));
                    _textValue = timeForSet.ToShortDateString();
                }

                var textPadding = (_textValue.Length * (_textSize * 0.5f)) / 2; //locate text in the center

                var replaceText = new RawRectangleF(leftPadding - textPadding, _chartDrawing.WindowHeight * (_chartDrawing.Paddings.PaddingBottomRatio + 0.025f), Render.Size.Width, Render.Size.Height);

                Render.DrawText(_textValue, TextFormat, replaceText, _chartDrawing.Brushes.Black);
                Render.DrawLine(vectorPoint1, vectorPoint2, _chartDrawing.Brushes.Black, 2);

            }
        }
    }
}
