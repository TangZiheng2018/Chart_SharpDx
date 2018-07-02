using System;
using SharpDxTest_WF.BarComponent.Models;
using SharpDxTest_WF.ChartRendering.Base;
using SharpDxTest_WF.ChartRendering.Helpers;
using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;

namespace SharpDxTest_WF.ChartRendering
{
    public class ChartDrawing : ChartWindowBase
    {
        #region Fields

        private float _partToResize;
        private TimingBy _dateIn;
        private BarSettings _barSettings;

        #endregion
        
        public ChartDrawing(float windowWidth, float windowHeight, RenderTarget render, TimingBy timeBy) : base(windowWidth, windowHeight, render)
        {
            BarInfo = new BarSettings(ChartHeight);
            AxeSetting = new AxeSetting(WindowSize, Paddings);

            DateIn = timeBy;
        }
        
        #region Properties

        public int CountBarsPerChart => _barSettings.CountBarsPerWindow;

        public float TimePerPixel => Convert.ToSingle(MinMaxValues.GetTimeDifference(DateIn) / (ChartWidth * 0.95));

        public BarSettings BarInfo
        {
            get => _barSettings;
            set => _barSettings = value ?? throw new NullReferenceException();
        }

        public float PartToResize
        {
            get => _partToResize;
            set => _partToResize = Math.Abs(value) > 0.0f ? value : throw new NullReferenceException();
        }

        public TimingBy DateIn { get => _dateIn; private set => _dateIn = default(TimingBy); }

        #endregion

        #region Methods

        public void UpdateMinMaxPoints(BarMinMaxPositions positions)
        {
            MinMaxValues.MinValueLocation = positions.MinValue;
            MinMaxValues.MaxValueLocation = positions.MaxValue;
            MinMaxValues.MinDateLocation = positions.MinDate;
            MinMaxValues.MaxDateLocation = positions.MaxDate;

            PartToResize = MinMaxValues.MaxValueLocation / CountBarsPerChart;
        }

        public void UpdateMinMaxValues(float minVall, float maxVall)
        {
            MinMaxValues.MinValueLocation = minVall;
            MinMaxValues.MaxValueLocation = maxVall;

            PartToResize = MinMaxValues.MaxValueLocation / CountBarsPerChart;
        }

        public void SetMinMaxBorders(BarMinMaxPositions positions)
        {
            MinMaxValues = new ChartAxesMinMaxValues(positions);
            PartToResize = MinMaxValues.MaxValueLocation / CountBarsPerChart;
            if (MinMaxValues == null)
                SetVectors();
        }

        public void ZoomChart(RawRectangleF zoomArea)
        {
            var startDatePerPx = zoomArea.Left - ChartWidth * Paddings.PaddingLeftRatio;
            var finishDatePerPx = zoomArea.Right - ChartWidth * Paddings.PaddingLeftRatio;

            var diff = MinMaxValues.MaxDateLocation.Subtract(MinMaxValues.MinDateLocation).TotalMinutes;
            var minutePerPixel = ChartWidth / diff;

            var countMinutesFrom = Math.Floor(startDatePerPx / minutePerPixel);
            var countMinutesTo = Math.Ceiling(finishDatePerPx / minutePerPixel);

            if (countMinutesFrom < 0 || countMinutesTo > diff)
                return;

            var from = MinMaxValues.MinDateLocation.AddMinutes(countMinutesFrom);
            var to = MinMaxValues.MinDateLocation.AddMinutes(countMinutesTo);


            var fromMinToTopPosition = WindowHeight * Paddings.PaddingBottomRatio - zoomArea.Top;
            var fromMinToBottomPosition = WindowHeight * Paddings.PaddingBottomRatio - zoomArea.Bottom;

            var diffValues = MinMaxValues.MaxValueLocation - MinMaxValues.MinValueLocation;
            var valPerHeight = diffValues / ChartHeight;

            var valueSecondPart = fromMinToBottomPosition * valPerHeight;
            var valueFirstPart = fromMinToTopPosition * valPerHeight;

            var value1 = MinMaxValues.MinValueLocation + valueFirstPart;
            var value2 = MinMaxValues.MinValueLocation + valueSecondPart;

            var positions = new BarMinMaxPositions(Math.Min(value1, value2), Math.Max(value1, value2), from, to);

            SetMinMaxBorders(positions);
        }

        public void ResizeUpDown(bool isToUp)
        {
            if (isToUp)
            {
                if (MinMaxValues.MinValueLocation - PartToResize <= 0)
                    return;

                MinMaxValues.MinValueLocation -= PartToResize;
                MinMaxValues.MaxValueLocation += PartToResize;
                return;
            }

            var max = MinMaxValues.MaxValueLocation - PartToResize;
            var min = MinMaxValues.MinValueLocation + PartToResize;

            if (min >= MinMaxValues.Positions.MaxValue)
                return;

            if (min < 0)
                return;

            if (max < min || max - min < PartToResize)
                return;

            MinMaxValues.MinValueLocation += PartToResize;
            MinMaxValues.MaxValueLocation -= PartToResize;
        }


        #endregion

    }
}
