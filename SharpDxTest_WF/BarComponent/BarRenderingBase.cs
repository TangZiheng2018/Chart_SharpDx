using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDxTest_WF.BarComponent.Models;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;

namespace SharpDxTest_WF.BarComponent
{
    public abstract class BarRenderingBase
    {
        #region fields

        private List<BarModel> _bars;
        private readonly RenderTarget _render;
        private readonly SolidColorBrush _redBgBrush;
        private readonly SolidColorBrush _greenBgBrush;
        private readonly SolidColorBrush _borderBlackBrush;
        private readonly ChartSettings _chartInfo;

        #endregion

        #region properties

        protected ChartSettings ChartInfo => _chartInfo;
        public List<BarModel> Bars => _bars;
        protected RenderTarget Render => _render;
        protected SolidColorBrush RedBgBrush => _redBgBrush;
        protected SolidColorBrush GreenBgBrush => _greenBgBrush;
        protected SolidColorBrush BorderBlackBrush => _borderBlackBrush;
        public BarsMinMaxPositions MinMaxPositions
        {
            get => SetMinMaxPositions();
        }
        public int Skip { get; set; }

        #endregion
        
        protected BarRenderingBase(RenderTarget render, List<BarModel> bars, ChartSettings chartInfo)
        {
            _bars = bars;
            _render = render;
            _chartInfo = chartInfo;
            _redBgBrush = new SolidColorBrush(_render, Color.Red);
            _greenBgBrush = new SolidColorBrush(_render, Color.Green);
            _borderBlackBrush = new SolidColorBrush(_render, Color.Black);
        }

        public abstract void RenderBars();

        protected virtual float GetYScreenPoint(float value, float windowHeight, float minValue, float maxValue)
        {
            var chartHeight = windowHeight * 0.8f;
            var normalize = (value - minValue) / (maxValue - minValue);
            return (chartHeight - (chartHeight * normalize)) + windowHeight * 0.1f;
        }

        protected BarsMinMaxPositions SetMinMaxPositions()
        {
            var borders = new BarsMinMaxPositions();
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

    }
}
