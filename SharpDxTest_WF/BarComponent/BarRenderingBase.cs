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

        protected virtual float GetYScreenPoint(float value, float windowHeight, float minValue, float maxValue)
        {
            var chartHeight = windowHeight * 0.8f;
            var normalize = (value - minValue) / (maxValue - minValue);
            return (chartHeight - (chartHeight * normalize)) + windowHeight * 0.1f;
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

    }
}
