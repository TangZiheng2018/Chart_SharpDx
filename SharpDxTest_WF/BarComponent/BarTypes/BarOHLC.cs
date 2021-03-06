﻿using System;
using System.Collections.Generic;
using System.Linq;
using SharpDxTest_WF.ChartRendering;
using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;

namespace SharpDxTest_WF.BarComponent.BarTypes
{
    public class BarOHLC : BarRenderingBase
    {
        public BarOHLC(RenderTarget render, List<BarModel> bars, ChartDrawing chartInfo, int skip = 0) : base(render, bars, chartInfo)
        {
            Skip = skip;
        }


        protected RawVector2 GetLineVector(float timeForLocate, float value)
        {
            var padding = ChartInfo.WindowWidth * ChartInfo.Paddings.PaddingLeftRatio;

            var timePointX = (timeForLocate / ChartInfo.TimePerPixel) + padding + ChartInfo.BarInfo.BarWidth / 2;

            var pointY = GetYScreenPoint(value, ChartInfo.WindowHeight, ChartInfo.MinMaxValues.MinValueLocation,
                ChartInfo.MinMaxValues.MaxValueLocation);

            return new RawVector2(timePointX, pointY * 0.99f);
        }

        protected RawRectangleF GetPlacement(float timeForLocate, float open, float close)
        {
            var padding = ChartInfo.WindowWidth * ChartInfo.Paddings.PaddingLeftRatio;

            var timePositionXAxe = (timeForLocate / ChartInfo.TimePerPixel) + padding;

            var closePoint = GetYScreenPoint(Convert.ToSingle(close), ChartInfo.WindowHeight,
                ChartInfo.MinMaxValues.MinValueLocation, ChartInfo.MinMaxValues.MaxValueLocation);

            var openPoint = GetYScreenPoint(Convert.ToSingle(open), ChartInfo.WindowHeight,
                ChartInfo.MinMaxValues.MinValueLocation, ChartInfo.MinMaxValues.MaxValueLocation);

            if (close > open)
                return new RawRectangleF(timePositionXAxe, closePoint,
                    timePositionXAxe + ChartInfo.BarInfo.BarWidth, openPoint);
            

            return new RawRectangleF(timePositionXAxe, openPoint,
                    timePositionXAxe + ChartInfo.BarInfo.BarWidth, closePoint);
        }
        
        public override void StartRendering()
        {
            foreach (var bar in Bars.Skip(Skip).Take(ChartInfo.CountBarsPerChart))
            {
                var timeSpan = GetTimeDifference(bar.Time);

                var rectangle = GetPlacement(timeSpan, (float)bar.Open, (float)bar.Close);
                
                Render.DrawLine(
                    GetLineVector(timeSpan, (float)bar.High),
                    GetLineVector(timeSpan, (float)bar.Low), ChartInfo.Brushes.Black, 2);

                Render.FillRectangle(rectangle, bar.IsBear == true ? ChartInfo.Brushes.Red : ChartInfo.Brushes.Green);
                Render.DrawRectangle(rectangle, ChartInfo.Brushes.Black);
            }

        }
    }
}
