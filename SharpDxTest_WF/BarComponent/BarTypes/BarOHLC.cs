using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;

namespace SharpDxTest_WF.BarComponent.BarTypes
{
    public class BarOHLC : BarRenderingBase
    {
        public BarOHLC(RenderTarget render, List<BarModel> bars, ChartSettings chartInfo) : base(render, bars, chartInfo)
        {
            Skip = 0;
        }


        protected RawVector2 GetLineVector(ChartSettings chartSettings, float timeForLocate, float value)
        {
            var padding = chartSettings.WindowWidth * chartSettings.PaddingLeftRatio;
            var barWidth = chartSettings.WindowWidth * chartSettings.BarWidthPercent;

            var timePointX = (timeForLocate / chartSettings.TimePerPixel) + padding + barWidth / 2;

            var pointY = GetYScreenPoint(value, chartSettings.WindowHeight, chartSettings.AxeValues.MinValueLocation,
                chartSettings.AxeValues.MaxValueLocation);

            return new RawVector2(timePointX, pointY);
        }

        protected RawRectangleF GetPlacement(ChartSettings chartSettings, float timeForLocate, float open, float close)
        {
            var padding = chartSettings.WindowWidth * chartSettings.PaddingLeftRatio;
            var barWidth = chartSettings.WindowWidth * chartSettings.BarWidthPercent;

            var timePositionXAxe = (timeForLocate / chartSettings.TimePerPixel) + padding;

            var closePoint = GetYScreenPoint(Convert.ToSingle(close), chartSettings.WindowHeight,
                chartSettings.AxeValues.MinValueLocation, chartSettings.AxeValues.MaxValueLocation);

            var openPoint = GetYScreenPoint(Convert.ToSingle(open), chartSettings.WindowHeight,
                chartSettings.AxeValues.MinValueLocation, chartSettings.AxeValues.MaxValueLocation);

            if (close > open)
                return new RawRectangleF(timePositionXAxe, closePoint,
                    timePositionXAxe + barWidth, openPoint);
            else
                return new RawRectangleF(timePositionXAxe, openPoint,
                    timePositionXAxe + barWidth, closePoint);
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
                        timeSpan = (int)currentBarTime.TotalMinutes;
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

                var sett = GetPlacement(ChartInfo, timeSpan, (float)bar.Open, (float)bar.Close);

                var rectangle = new RawRectangleF(sett.Left, sett.Top, sett.Right, sett.Bottom);

                Render.DrawLine(
                    GetLineVector(ChartInfo, timeSpan, (float)bar.High),
                    GetLineVector(ChartInfo, timeSpan, (float)bar.Low), ChartInfo.Brushes.Black, 2);

                Render.FillRectangle(rectangle, bar.IsBear == true ? ChartInfo.Brushes.Red : ChartInfo.Brushes.Green);
                Render.DrawRectangle(rectangle, ChartInfo.Brushes.Black);
            }
        }
    }
}
