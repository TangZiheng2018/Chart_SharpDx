using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpDxTest_WF.ChartRendering.Interfaces
{
    public class BarSettings
    {

        #region Fields

        private float _barWidthPercent = 0.04f;
        private float _spaceBetweenBarsPercent = 0.02f;
        private float _windowWidth;

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

        public float BarWidth
        {
            get => _barWidthPercent * _windowWidth;
        }

        public float SpaceBetweenBars
        {
            get => _spaceBetweenBarsPercent * _windowWidth;
        }

        public int CountBarsPerWindow
        {
            get => Convert.ToInt32((_windowWidth / (BarWidth + SpaceBetweenBars)) * 0.9f); // take 90 percent, to not overflow window
        }

        #endregion

        #region Constructors

        public BarSettings(float barWidthPercent, float spaceBetweenBarsPercent, float chartWidth)
        {
            BarWidthPercent = barWidthPercent;
            SpaceBetweenBarsPercent = spaceBetweenBarsPercent;
            _windowWidth = chartWidth;
        }


        #endregion

    }
}
