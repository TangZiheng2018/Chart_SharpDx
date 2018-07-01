using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDxTest_WF.BarComponent.Models;
using SharpDxTest_WF.ChartRendering;
using SharpDxTest_WF.Models;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;

namespace SharpDxTest_WF.BarComponent
{
    public abstract class BarRenderingBase : RenderingBase
    {
        #region Fields

        private List<BarModel> _bars;
        private SolidColorBrush _redBgBrush;
        private SolidColorBrush _greenBgBrush;
        private SolidColorBrush _borderBlackBrush;
        private ChartDrawing _chartInfo;

        #endregion

        #region Properties

        protected ChartDrawing ChartInfo
        {
            get => _chartInfo;
            set => _chartInfo = value ?? throw new NullReferenceException();
        }

        public List<BarModel> Bars
        {
            get => _bars;
            set
            {
                if(value?.Any() == true)
                    _bars = value;
                throw new NullReferenceException();
            }
        }

        protected SolidColorBrush RedBgBrush
        {
            get => _redBgBrush;
            set => _redBgBrush = value ?? throw new NullReferenceException();
        }

        protected SolidColorBrush GreenBgBrush
        {
            get => _greenBgBrush;
            set => _greenBgBrush = value ?? throw new NullReferenceException();
        }

        protected SolidColorBrush BorderBlackBrush
        {
            get => _borderBlackBrush;
            set => _borderBlackBrush = value ?? throw new NullReferenceException();
        }
        
        public int Skip { get; set; }

        #endregion
        
        protected BarRenderingBase(RenderTarget render, List<BarModel> bars, ChartDrawing chartInfo) : base(render)
        {
            _bars = bars;
            _chartInfo = chartInfo;
            RedBgBrush = new SolidColorBrush(Render, Color.Red);
            GreenBgBrush = new SolidColorBrush(Render, Color.Green);
            BorderBlackBrush = new SolidColorBrush(Render, Color.Black);
        }

        #region Methods

        protected virtual float GetYScreenPoint(float value, float windowHeight, float minValue, float maxValue)
        {
            var chartHeight = windowHeight * 0.8f;
            var normalize = (value - minValue) / (maxValue - minValue);
            return (chartHeight - (chartHeight * normalize)) + windowHeight * 0.1f;
        }

        protected int GetTimeDifference(DateTime time)
        {
            var currentBarTime = time.Subtract(ChartInfo.MinMaxValues.MinDateLocation);

            switch (ChartInfo.DateIn)
            {
                case TimingBy.Minute:
                    return Convert.ToInt32(currentBarTime.TotalMinutes);
                    break;
                case TimingBy.Hour:
                    return Convert.ToInt32(currentBarTime.TotalHours);
                    break;
                case TimingBy.Day:
                    return Convert.ToInt32(currentBarTime.TotalDays);
                    break;
                default:
                    return 0;
            }
        }

        public BarMinMaxPositions MinMaxPositions()
        {
            var borders = new BarMinMaxPositions();
            var take = _chartInfo.CountBarsPerChart;

            borders.MinValue = Convert.ToSingle(_bars
                .Skip(Skip)
                .Take(take)
                .Min(x => x.Low));

            borders.MaxValue = Convert.ToSingle(_bars
                .Skip(Skip)
                .Take(take)
                .Max(x => x.High));

            borders.MinDate = _bars
                .Skip(Skip)
                .Take(take)
                .Min(x => x.Time);

            borders.MaxDate = _bars
                .Skip(Skip)
                .Take(take)
                .Max(x => x.Time);

            return borders;
        }

        public void RenderLastPosition()
        {
            var lastBar = Bars.Skip(Skip).Take(ChartInfo.CountBarsPerChart).Last();

            var timeSpan = GetTimeDifference(lastBar.Time);

            var yPosition = GetYScreenPoint((float)lastBar.LastPrice, ChartInfo.WindowHeight,
                ChartInfo.MinMaxValues.MinValueLocation, ChartInfo.MinMaxValues.MaxValueLocation);

            var xPosition = ChartInfo.AxeSetting.TouchMiddlePoint.X;

            var firstVector = new RawVector2(xPosition - ChartInfo.ChartWidth * 0.02f, yPosition);
            var lastVector = new RawVector2(xPosition, yPosition);

            var offset = ChartInfo.AxeSetting.PointOnEveryPercentAxeY * ChartInfo.ChartHeight / 5;

            var rectangle = new RoundedRectangle
            {
                Rect = new RawRectangleF(xPosition - offset, yPosition - offset, xPosition + ChartInfo.ChartHeight * 0.05f,
                    yPosition + offset),
                RadiusX = 1,
                RadiusY = 1
            };

            Render.FillRoundedRectangle(rectangle, ChartInfo.Brushes.Red);
            Render.DrawLine(firstVector, lastVector, ChartInfo.Brushes.Black, 3);
        }

        #endregion

    }
}
