using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDxTest_WF.BarComponent.Models;
using SharpDxTest_WF.ChartRendering;
using SharpDxTest_WF.DrawingsComponent.AdditionalModels;
using SharpDX.Direct2D1;
using Text = SharpDX.DirectWrite.TextFormat;
using SharpDX.Mathematics.Interop;

namespace SharpDxTest_WF.BarComponent.BarTypes
{
    public class BarCandle : BarRenderingBase
    {
        public BarCandle(RenderTarget render, List<BarModel> bars, ChartDrawing chartInfo, int skip = 0) : base(render, bars, chartInfo)
        {
            Skip = skip;
        }
        

        protected RawVector2 GetVector(ChartDrawing chartSettings, float timeForLocate, float value)
        {
            var padding = chartSettings.WindowWidth * chartSettings.Paddings.PaddingLeftRatio;

            var timePointX = (timeForLocate / chartSettings.TimePerPixel) + padding + chartSettings.BarInfo.BarWidth / 2;

            var pointY = GetYScreenPoint(value, chartSettings.WindowHeight, chartSettings.MinMaxValues.MinValueLocation,
                chartSettings.MinMaxValues.MaxValueLocation);

            return new RawVector2(timePointX, pointY * 0.99f);
        }

        protected RawVector2 GetPlacement(ChartDrawing chartSettings, float timeForLocate, float value, bool isStart = false, bool isOpenBar = true)
        {
            var padding = chartSettings.WindowWidth * chartSettings.Paddings.PaddingLeftRatio;
            var left = ChartInfo.BarInfo.BarWidth / 2;

            var timePointX = (timeForLocate / chartSettings.TimePerPixel) + padding + left;

            if (isStart && isOpenBar)
                timePointX -= left;
            if (!isStart && !isOpenBar)
                timePointX += left;

            var valueYPoint = GetYScreenPoint(value, chartSettings.WindowHeight,
                chartSettings.MinMaxValues.MinValueLocation, chartSettings.MinMaxValues.MaxValueLocation);

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

                var vectorOpenStart = GetPlacement(ChartInfo, timeSpan, open, true);
                var vectorCloseStart = GetPlacement(ChartInfo, timeSpan, close, true, false);

                var vectorOpenFinish = GetPlacement(ChartInfo, timeSpan, open);
                var vectorCloseFinish = GetPlacement(ChartInfo, timeSpan, close, isOpenBar: false);

                Render.DrawLine(vectorOpenStart, vectorOpenFinish,
                    GreenBgBrush, 2);
                Render.DrawLine(vectorCloseStart, vectorCloseFinish,
                    RedBgBrush, 2);


                var vectorHigh = GetVector(ChartInfo, timeSpan, (float)bar.High);
                var vectorLow = GetVector(ChartInfo, timeSpan, (float)bar.Low);

                Render.DrawLine(vectorHigh, vectorLow, ChartInfo.Brushes.Black, 2);
            }

            RenderLastPosition();
        }
    }
}
