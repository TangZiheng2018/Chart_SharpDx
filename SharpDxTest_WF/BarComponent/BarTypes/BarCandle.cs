using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDxTest_WF.BarComponent.Models;
using SharpDxTest_WF.DrawingsComponent.AdditionalModels;
using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;

namespace SharpDxTest_WF.BarComponent.BarTypes
{
    public class BarCandle : BarRenderingBase
    {
        public BarCandle(RenderTarget render, List<BarModel> bars, ChartSettings chartInfo) : base(render, bars, chartInfo)
        {
            Skip = 0;
        }
        
        protected RawVector2 GetVector(ChartSettings chartSettings, float timeForLocate, float value)
        {
            var padding = chartSettings.WindowWidth * chartSettings.PaddingLeftRatio;
            var barWidth = chartSettings.WindowWidth * chartSettings.BarWidthPercent;

            var timePointX = (timeForLocate / chartSettings.TimePerPixel) + padding + barWidth / 2;

            var pointY = GetYScreenPoint(value, chartSettings.WindowHeight, chartSettings.AxeValues.MinValueLocation,
                chartSettings.AxeValues.MaxValueLocation);

            return new RawVector2(timePointX, pointY);
        }

        protected RawVector2 GetPlacement(ChartSettings chartSettings, float timeForLocate, float value, bool isStart = false, bool isOpenBar = true)
        {
            var padding = chartSettings.WindowWidth * chartSettings.PaddingLeftRatio;
            var offset = chartSettings.WindowWidth * 0.007f;

            var timePointX = (timeForLocate / chartSettings.TimePerPixel) + padding + offset;

            if (isStart && isOpenBar)
                timePointX -= offset;
            if (!isStart && !isOpenBar)
                timePointX += offset;

            var valueYPoint = GetYScreenPoint(value, chartSettings.WindowHeight,
                chartSettings.AxeValues.MinValueLocation, chartSettings.AxeValues.MaxValueLocation);

            return new RawVector2(timePointX, valueYPoint);
        }

        public override void RenderBars()
        {
            foreach (var bar in Bars.Skip(Skip).Take(ChartInfo.CountBarsPerChart))
            {
                var timeSpan = 0;
                var currentBarTime = bar.Time.Subtract(ChartInfo.AxeValues.MinDate);

                switch (ChartInfo.TimeIn)
                {
                    case TimingBy.Minute:
                        timeSpan = Convert.ToInt32(currentBarTime.TotalMinutes);
                        break;
                    case TimingBy.Hour:
                        timeSpan = Convert.ToInt32(currentBarTime.TotalHours);
                        break;
                    case TimingBy.Day:
                        timeSpan = Convert.ToInt32(currentBarTime.TotalDays);
                        break;
                    default:
                        timeSpan = Convert.ToInt32(currentBarTime.TotalDays);
                        break;
                }

                var open = (float)bar.Open;
                var close = (float)bar.Close;

                var vectorOpenStart = GetPlacement(ChartInfo, timeSpan, open, true);
                var vectorCloseStart = GetPlacement(ChartInfo, timeSpan, close , true,false);

                var vectorOpenFinish = GetPlacement(ChartInfo, timeSpan, open);
                var vectorCloseFinish = GetPlacement(ChartInfo, timeSpan, close, isOpenBar:false);

                Render.DrawLine(vectorOpenStart, vectorOpenFinish,
                    GreenBgBrush,2);
                Render.DrawLine(vectorCloseStart, vectorCloseFinish,
                    RedBgBrush,2);


                var vectorHigh = GetVector(ChartInfo, timeSpan, (float)bar.High);
                var vectorLow = GetVector(ChartInfo, timeSpan, (float)bar.Low);

                Render.DrawLine(vectorHigh, vectorLow, ChartInfo.Brushes.Black, 2);
            }
        }

        
    }
}
