using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDxTest_WF.ChartRendering.Helpers;

namespace SharpDxTest_WF.ChartRendering.Base
{
    public abstract class ChartWindowBase : WindowBase
    {
        #region Fields

        private float _chartWidth;
        private float _chartHeight;
        private Paddings _paddings;


        #endregion

        #region Properties

        public float ChartWidth
        {
            get => _chartWidth;
            set => _chartWidth = value;
        }

        public float ChartHeight
        {
            get => _chartHeight;
            set => _chartHeight = value;
        }

        public Paddings Paddings { get => _paddings; set => _paddings = value; }

        #endregion

        #region Constructors

        protected ChartWindowBase(float windowWidth, float windowHeight) : base(windowWidth, windowHeight)
        {

        }

        protected ChartWindowBase(Size windowSize) : base(windowSize)
        {
        }

        #endregion

        #region Methods
        
        protected void SetPadding(float padding)
        {
            _paddings = new Paddings(padding, padding, 1 - padding, 1 - padding);
            SetChartSize();
        }

        protected void SetChartSize()
        {
            ChartWidth = WindowWidth * (_paddings.PaddingRightRatio - _paddings.PaddingLeftRatio);
            ChartHeight = WindowHeight * (_paddings.PaddingBottomRatio - _paddings.PaddingTopRatio);
        }

        #endregion
    }
}
