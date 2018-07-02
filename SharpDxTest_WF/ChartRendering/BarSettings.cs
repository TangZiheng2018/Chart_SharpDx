using System;

namespace SharpDxTest_WF.ChartRendering
{
    public class BarSettings
    {

        #region Fields

        private float _barWidthPercent = 0.03f;
        private float _spaceBetweenBarsPercent = 0.04f;
        private readonly float _windowWidth;

        #endregion

        #region Properties

        public float BarWidthPercent
        {
            get => _barWidthPercent;
            set => _barWidthPercent = Math.Abs(value) > 0 ? value : throw new NullReferenceException();
        }

        public float SpaceBetweenBarsPercent
        {
            get => _spaceBetweenBarsPercent;
            set => _spaceBetweenBarsPercent = Math.Abs(value) > 0 ? value : throw new NullReferenceException();
        }

        public float BarWidth => _barWidthPercent * _windowWidth;

        public float SpaceBetweenBars => _spaceBetweenBarsPercent * _windowWidth;

        public int CountBarsPerWindow => Convert.ToInt32(_windowWidth / (BarWidth + SpaceBetweenBars) * 0.9f);

        #endregion

        #region Constructors

        public BarSettings(float chartWidth)
        {
            _windowWidth = chartWidth;
        }

        public BarSettings(float barWidthPercent, float spaceBetweenBarsPercent, float chartWidth)
        {
            BarWidthPercent = barWidthPercent;
            SpaceBetweenBarsPercent = spaceBetweenBarsPercent;
            _windowWidth = chartWidth;
        }
        
        #endregion

    }
}
