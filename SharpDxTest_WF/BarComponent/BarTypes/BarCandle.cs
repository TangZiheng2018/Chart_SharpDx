﻿using System.Collections.Generic;
using System.Linq;
using SharpDxTest_WF.ChartRendering;
using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;

namespace SharpDxTest_WF.BarComponent.BarTypes
{
    public class BarCandle : BarRenderingBase
    {
        public BarCandle(RenderTarget render, List<BarModel> bars, ChartDrawing chartInfo, int skip = 0) : base(render, bars, chartInfo)
        {
            Skip = skip;
        }
        

        protected RawVector2 GetVector(float timeForLocate, float value)
        {
            var padding = ChartInfo.WindowWidth * ChartInfo.Paddings.PaddingLeftRatio;

            var timePointX = (timeForLocate / ChartInfo.TimePerPixel) + padding + ChartInfo.BarInfo.BarWidth / 2;

            var pointY = GetYScreenPoint(value, ChartInfo.WindowHeight, ChartInfo.MinMaxValues.MinValueLocation,
                ChartInfo.MinMaxValues.MaxValueLocation);

            return new RawVector2(timePointX, pointY * 0.99f);
        }

        protected RawVector2 GetPlacement(float timeForLocate, float value, bool isStart = false, bool isOpenBar = true)
        {
            var padding = ChartInfo.WindowWidth * ChartInfo.Paddings.PaddingLeftRatio;
            var offset = ChartInfo.BarInfo.BarWidth / 2;

            var timePointX = (timeForLocate / ChartInfo.TimePerPixel) + padding + offset;

            if (isStart && isOpenBar)
                timePointX -= offset * 1.5f;
            if (!isStart && !isOpenBar)
                timePointX += offset * 1.5f;

            var valueYPoint = GetYScreenPoint(value, ChartInfo.WindowHeight,
                ChartInfo.MinMaxValues.MinValueLocation, ChartInfo.MinMaxValues.MaxValueLocation);

            return new RawVector2(timePointX, valueYPoint);
        }
        
        public override void StartRendering()
        {
            var bars = Bars.Skip(Skip).Take(ChartInfo.CountBarsPerChart).ToList();
            foreach (var bar in bars)
            {
                var open = (float)bar.Open;
                var close = (float)bar.Close;

                var timeSpan = GetTimeDifference(bar.Time);

                var vectorOpenStart = GetPlacement(timeSpan, open, true);
                var vectorCloseStart = GetPlacement(timeSpan, close, true, false);

                var vectorOpenFinish = GetPlacement(timeSpan, open);
                var vectorCloseFinish = GetPlacement(timeSpan, close, isOpenBar: false);

                Render.DrawLine(vectorOpenStart, vectorOpenFinish,
                    GreenBgBrush, 3);
                Render.DrawLine(vectorCloseStart, vectorCloseFinish,
                    RedBgBrush, 3);


                var vectorHigh = GetVector( timeSpan, (float)bar.High);
                var vectorLow = GetVector(timeSpan, (float)bar.Low);

                Render.DrawLine(vectorHigh, vectorLow, ChartInfo.Brushes.Black, 2);
            }
        }
    }
}
