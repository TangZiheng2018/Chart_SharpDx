using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDxTest_WF.BarComponent.Models;
using SharpDX.Direct3D11;
using Color = SharpDX.Color;

namespace SharpDxTest_WF
{
    public class ChartSettings
    {

        #region Construstors
        public ChartSettings(float width, float heigth, RenderTarget renderTarget, TimingBy timingBy, float barWidth = 0.02f, float spaceBetweendBars = 0.01f)
        {
            _windowWidth = width;
            _windowHeight = heigth;
            TimeIn = timingBy;


            SetBrushes(renderTarget);
            SetBarSettings(barWidth, spaceBetweendBars);
            SetPadding(0.1f);
        }

        public ChartSettings(float width, float heigth, float paddings)
        {
            _windowWidth = width;
            _windowHeight = heigth;
            
            SetBarSettings(0.02f, 0.01f);
            SetPadding(paddings);
        }

        public ChartSettings(float width, float heigth, float xAxe, float yAxe)
        {
            _windowWidth = width;
            _windowHeight = heigth;

            SetBarSettings(0.02f, 0.01f);
            SetPadding(xAxe,yAxe);
            SetChartSize();
        }
        #endregion

        #region PrivateFields
        private float _windowWidth;
        private float _windowHeight;
        private float _chartWidth;
        private float _chartHeight;
        private float _paddingLeftRatio;
        private float _paddingRightRatio;
        private float _paddingTopRatio;
        private float _paddingBottomRatio;
        private float _barWidthPercent = 0.04f;
        private float _spaceBetweenBarsPercent = 0.02f;
        private int _countBarsPerChart;
        private float _partToResize;
        private TimingBy _dateIn;
        #endregion
         
        #region Properties
        public float Width
        {
            get
            {
                return _chartWidth;
            }
            set
            {
                if (value > 0)
                {
                    _chartWidth = value;
                }
            }
        }

        public float Height
        {
            get
            {
               return _chartHeight;
            }
            set
            {
                if (value > 0)
                {
                    _chartHeight = value;
                }
            }
        }

        public float WindowWidth
        {
            get
            {
                return _windowWidth;
            }
            set
            {
                if (value > 0)
                {
                    _windowWidth = value;
                }
            }
        }

        public float WindowHeight
        {
            get
            {
                return _windowHeight;
            }
            set
            {
                if (value > 0)
                {
                    _windowHeight = value;
                }
            }
        }

        public float PaddingTopRatio
        {
            get => _paddingTopRatio;
            set => _paddingTopRatio = Math.Abs(value) > 0 ? value : throw new NullReferenceException();
        }

        public float PaddingBottomRatio
        {
            get => _paddingBottomRatio;
            set => _paddingBottomRatio = Math.Abs(value) > 0 ? value : throw new NullReferenceException();
        }

        public float PaddingRightRatio
        {
            get => _paddingRightRatio;
            set => _paddingRightRatio = Math.Abs(value) > 0 ? value : throw new NullReferenceException();
        }

        public float PaddingLeftRatio
        {
            get => _paddingLeftRatio;
            set => _paddingLeftRatio = Math.Abs(value) > 0 ? value : throw new NullReferenceException();
        }

        public float BarWidthPercent
        {
            get => _barWidthPercent;
            set => _barWidthPercent = Math.Abs(value) > 0 ? value : throw new NullReferenceException();
        }

        public float PartToIncrease
        {
            get => _partToResize;
            set => _partToResize = value; 

        }
        
        public float SpaceBetweenBarsPercent
        {
            get => _spaceBetweenBarsPercent;
            set => _spaceBetweenBarsPercent = Math.Abs(value) > 0 ? value : throw new NullReferenceException();
        }

        public int CountBarsPerChart
        {
            get => Convert.ToInt32((_chartWidth / (_barWidthPercent * _chartWidth + _spaceBetweenBarsPercent * _chartWidth))*0.9f); // get less on 10% bars, to make chart not overflowed
        }

        public TimingBy TimeIn
        {
            get => _dateIn;
            set => _dateIn = value >= 0 ? value : throw new IndexOutOfRangeException();
        }

        public float ValuePerPixel
        {
            get => AxeValues.MaxValueLocation / Height;
        }

        public float TimePerPixel
        {
            get
            {
                double time = 0;
                switch(TimeIn)
                {
                    case TimingBy.Minute:
                        {
                            time = AxeValues.MaxDateLocation.Subtract(AxeValues.MinDateLocation).TotalMinutes;
                        }
                        break;

                    case TimingBy.Hour:
                        {
                            time = AxeValues.MaxDateLocation.Subtract(AxeValues.MinDateLocation).TotalHours;
                        }
                        break;

                    case TimingBy.Day:
                        {
                            time = AxeValues.MaxDateLocation.Subtract(AxeValues.MinDateLocation).TotalDays;
                        }
                    break;

                    case TimingBy.Week:
                        {
                            time = AxeValues.MaxDate.Subtract(AxeValues.MinDate).TotalDays/7;
                        }
                    break;

                    case TimingBy.Mouth:
                        {
                            time = AxeValues.MaxDate.Subtract(AxeValues.MinDate).TotalDays/30;
                        }
                    break;

                    case TimingBy.Year:
                        {
                            time = AxeValues.MaxDate.Subtract(AxeValues.MinDate).TotalDays/365;
                        }
                    break;

                    default: throw new ArgumentOutOfRangeException();

                }
                return Convert.ToSingle(time / (Width  * 0.95)); //place for padding
            }
        }

        public ChartMinMaxValues AxeValues { get; private set; }

        public float TouchAxesPosition
        {
            get => AxeSettings.touchXYPoint.Y;
        }

        public AxeSettings AxeSettings { get; private set; }

        public Brushes Brushes { get; private set; }

        #endregion

        #region Methods
        public void SetPadding(float left, float top, float right, float bottom)
        {
            PaddingLeftRatio = left;
            PaddingTopRatio = top;
            PaddingRightRatio = 1-right;
            PaddingBottomRatio = 1 - bottom;
            SetChartSize();
        }

        public void SetPadding(float xAxe, float yAxe)
        {
            PaddingLeftRatio = xAxe;
            PaddingTopRatio = yAxe;
            PaddingRightRatio = 1 - xAxe;
            PaddingBottomRatio = 1 - yAxe;
            SetChartSize();
        }

        public void SetPadding(float paddings)
        {
            PaddingLeftRatio = paddings;
            PaddingTopRatio = paddings;
            PaddingRightRatio = 1 - paddings;
            PaddingBottomRatio = 1 - paddings;
            SetChartSize();
        }

        public void UpdateChartToResize(Size newChartSize)
        {
            WindowHeight = newChartSize.Height;
            WindowWidth = newChartSize.Width;
            SetChartSize();
            SetVectors();
        }

        public void ZoomChart(RawRectangleF zoomArea)
        {
            var startDatePerPx = zoomArea.Left - WindowWidth * PaddingLeftRatio;
            var finishDatePerPx = zoomArea.Right - WindowWidth * PaddingLeftRatio;
            var diff = 0.0; 

            switch (TimeIn)
            {
                case TimingBy.Minute:
                    diff = AxeValues.MaxDateLocation.Subtract(AxeValues.MinDateLocation).TotalMinutes;
                    break;
                case TimingBy.Hour:
                    diff = AxeValues.MaxDateLocation.Subtract(AxeValues.MinDateLocation).TotalHours;
                    break;
                case TimingBy.Day:
                    diff = AxeValues.MaxDateLocation.Subtract(AxeValues.MinDateLocation).TotalDays;
                    break;
            }

            var timePerPixel = Width / diff;

            var countTimeFrom = Math.Floor(startDatePerPx / timePerPixel);
            var countTimeTo = Math.Ceiling(finishDatePerPx / timePerPixel);

            if (countTimeFrom < 0 || countTimeTo > diff)
                return;

            var timeFrom = AxeValues.MinDateLocation.AddDays(countTimeFrom);
            var timeTo = AxeValues.MinDateLocation.AddDays(countTimeTo);


            var fromMinToTopPosition = WindowHeight * PaddingBottomRatio - zoomArea.Top;
            var fromMinToBottomPosition = WindowHeight * PaddingBottomRatio - zoomArea.Bottom;

            var diffValues = AxeValues.MaxValueLocation - AxeValues.MinValueLocation;
            var valPerHeight = diffValues / Height;

            var valueSecondPart = fromMinToBottomPosition * valPerHeight;
            var valueFirstPart = fromMinToTopPosition * valPerHeight;

            var value1 = AxeValues.MinValueLocation + valueFirstPart;
            var value2 = AxeValues.MinValueLocation + valueSecondPart;

            SetMinMaxBorders(Min(value1, value2), Max(value1, value2), timeFrom, timeTo);
        }

        private float Min(float x, float y)
        {
            if (x < y)
                return x;
            return y;
        }

        private float Max(float x, float y)
        {
            if (x > y)
                return x;
            return y;
        }
        
        private void SetChartSize()
        {
            Width = _windowWidth * (_paddingRightRatio-_paddingLeftRatio);
            Height = _windowHeight * (_paddingBottomRatio- _paddingTopRatio);
        }
        
        public void SetBarSettings(float barWidthPercent, float spaceBetweenBarsPercent)
        {
            BarWidthPercent = barWidthPercent;
            SpaceBetweenBarsPercent = spaceBetweenBarsPercent;
        }

        public void SetMinMaxBorders(float minVall, float maxVall, DateTime minDate, DateTime maxDate)
        {
            AxeValues = new ChartMinMaxValues(minVall,maxVall,minDate,maxDate);
            _partToResize = AxeValues.MaxValue / CountBarsPerChart;
            if (AxeSettings == null)
                SetVectors();
        }
        public void SetMinMaxBorders(BarsMinMaxPositions positions)
        {
            AxeValues = new ChartMinMaxValues(positions.MinValue, positions.MaxValue, positions.MinDate, positions.MaxDate);
            _partToResize = AxeValues.MaxValue / CountBarsPerChart;
            if (AxeSettings == null)
                SetVectors();
        }

        public void UpdateMinMaxValues(float minVall, float maxVall)
        {
            AxeValues.MinValue = minVall;
            AxeValues.MaxValue = maxVall;
            
            _partToResize = AxeValues.MaxValue / CountBarsPerChart;
        }

        public void UpdateMinMaxPoints(float minVall, float maxVall, DateTime minDate, DateTime maxDate)
        {
            AxeValues.MinValue = minVall;
            AxeValues.MaxValue = maxVall;
            AxeValues.MinDate = minDate;
            AxeValues.MaxDate = maxDate;

            _partToResize = AxeValues.MaxValue / CountBarsPerChart;
        }

        public void SetVectors ()
        {
            AxeSettings = new AxeSettings(_windowHeight, _windowWidth, _paddingLeftRatio, _paddingTopRatio, _paddingRightRatio, _paddingBottomRatio, ValuePerPixel);
        }

        public Vector2[] XAxeVectors()
        {
            return new[] { AxeSettings.xLeftPoint, AxeSettings.touchXYPoint };
        }

        public Vector2[] YAxeVectors()
        {
            return new[] { AxeSettings.yTopPoint, AxeSettings.touchXYPoint };
        }

        public void SetBrushes(RenderTarget renderTarget)
        {
            Brushes = new Brushes(renderTarget);
        }

        public void ResizeUp()
        {
            if (AxeValues.MinValue - _partToResize <= 0)
                return;

            AxeValues.MinValueLocation -= _partToResize;
            AxeValues.MaxValueLocation += _partToResize;
        }

        public void ResizeDown()
        {
            var max = AxeValues.MaxValueLocation - _partToResize;
            var min = AxeValues.MinValueLocation + _partToResize;

            if ( min >= AxeValues.MaxValue)
                return;
            if (min < 0)
                return;
            if ( max < min || max-min < _partToResize)
                return;

            AxeValues.MinValueLocation += _partToResize;
            AxeValues.MaxValueLocation -= _partToResize;
        }

        #endregion

    }

    public class Brushes
    {
        public SolidColorBrush Red { get; }
        public SolidColorBrush Green { get; }
        public SolidColorBrush Black { get; }
        public SolidColorBrush TransparentBlack { get; }
        
        public Brushes(RenderTarget renderTarget)
        {
            Red = new SolidColorBrush(renderTarget,Color.Red);
            Green = new SolidColorBrush(renderTarget, Color.LimeGreen);
            Black = new SolidColorBrush(renderTarget, Color.Black);
            TransparentBlack = new SolidColorBrush(renderTarget, Color.Black, new BrushProperties { Opacity = 0.25f });
        }
        
    }
        
    public class ChartMinMaxValues
    {
        public ChartMinMaxValues(float minVall, float maxVall, DateTime minDate, DateTime maxDate)
        {
            MaxValue = maxVall;
            MinValue = minVall;
            MaxDate = maxDate;
            MinDate = minDate;
        }
        
        private float _minValue;
        private float _maxValue;
        private DateTime _minDate;
        private DateTime _maxDate;

        public float MinValue
        {
            get => _minValue;
            set
            {
                _minValue = value >= 0 ? value : throw new NullReferenceException();
                MinValueLocation = value;
            }
        }

        public float MaxValue
        {
            get => _maxValue;
            set
            {
                _maxValue = value >= 0 ? value : throw new NullReferenceException();
                MaxValueLocation = value;
            }
        }

        public DateTime MinDate
        {
            get => _minDate;
            set
            {
                _minDate = value > DateTime.MinValue ? value : throw new NullReferenceException();
                MinDateLocation = value;
            }
        }

        public DateTime MaxDate
        {
            get => _maxDate;
            set
            {
                _maxDate = value < DateTime.MaxValue ? value : throw new OverflowException();
                MaxDateLocation = value;
            }
        }
        
        //variable which we could change while resizing or zooming and we can save origin max/min vars
        public float MinValueLocation { get; set; }

        public float MaxValueLocation { get; set; }

        public DateTime MinDateLocation { get; set; }

        public DateTime MaxDateLocation { get; set; }

    }

    public class AxeSettings
    {
        
        public Vector2 touchXYPoint;
        public Vector2 xLeftPoint;
        public Vector2 yTopPoint;

        //private float setPointOnEveryPercent;

        public float PointOnEveryPercentAxeX
        {
            get;
            private set;
        }
        public float PointOnEveryPercentAxeY

        {
            get;
            private set;
        }
        public float CountPointsOnYAxe
        {
            get;
            private set;
        }

        public float CountPointsOnXAxe
        {
            get;
            private set;
        }

        public float yValuePointStart;
        public float yValuePointFinish;

        public float xDatePointStart;
        public float xDatePointFinish;
        //public Vector2 xDatePointStart;
        //public Vector2 xDatePointFinish;


        public AxeSettings(float windowHeight, float windowWidth, float paddingLeft, float paddingTop, float paddingRight, float paddingBottom, float valuePerPixel)
        {
            touchXYPoint = new Vector2(windowWidth * (paddingRight), windowHeight * paddingBottom);
            xLeftPoint = new Vector2(windowWidth * (paddingLeft - 0.05f), windowHeight * paddingBottom);
            yTopPoint = new Vector2(windowWidth * paddingRight, windowHeight * (paddingTop - 0.05f));

            yValuePointStart = windowWidth * paddingRight;
            yValuePointFinish = windowWidth * (paddingRight + 0.02f);

            xDatePointStart = windowHeight * paddingBottom;
            xDatePointFinish = windowHeight * (paddingBottom + 0.02f);

            PointOnEveryPercentAxeY = 0.05f;
            PointOnEveryPercentAxeX = 0.1f;

            var chartHeight = windowHeight * (paddingBottom-paddingTop);
            var chartWidth = windowWidth * (paddingRight - paddingLeft);

            CountPointsOnYAxe = chartHeight / (chartHeight * PointOnEveryPercentAxeY);
            CountPointsOnXAxe = chartWidth / (chartWidth * PointOnEveryPercentAxeX);
        }

        public void CreateAxeXPoints(float valuePerPx)
        {
            

        }


        public void CreateAxeYPoints(float valuePerPx)
        {
            

        }

        public void SetPointOnEveryPercent(float percent)
        {
            PointOnEveryPercentAxeY = percent;
        }

    }

    public enum TimingBy
    {
        Second,
        Minute,
        Hour,
        Day,
        Week,
        Mouth,
        Year
    }
}
