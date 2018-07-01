using System;
using SharpDxTest_WF.ChartRendering.Helpers;
using SharpDX;
using SharpDX.Direct2D1;

namespace SharpDxTest_WF.ChartRendering.Base
{
    public abstract class ChartWindowBase : WindowBase
    {
        #region Fields

        private float _chartWidth;
        private float _chartHeight;
        private Color _chartColor;
        private Paddings _paddings;
        private ChartAxesMinMaxValues _minMaxValues;
        private AxeSetting _axeSetting;
        private Brushes _brushes;

        #endregion

        #region Properties

        public Color ChartColor => _chartColor;

        public float ChartWidth
        {
            get => _chartWidth;
            protected set => _chartWidth = Math.Abs(value) > 0.0f ? value : throw new NullReferenceException();
        }

        public float ChartHeight
        {
            get => _chartHeight;
            protected set => _chartHeight = Math.Abs(value) > 0.0f ? value : throw new NullReferenceException();
        }

        public Paddings Paddings
        {
            get => _paddings;
            protected set => _paddings = value ?? throw new NullReferenceException();
        }

        public ChartAxesMinMaxValues MinMaxValues
        {
            get => _minMaxValues;
            protected set => _minMaxValues = value ?? throw new NullReferenceException();
        }

        public AxeSetting AxeSetting
        {
            get => _axeSetting;
            protected set => _axeSetting = value ?? throw new NullReferenceException();
        }

        public Brushes Brushes
        {
            get => _brushes;
            protected set => _brushes = value ?? throw new NullReferenceException();
        }

        #endregion
        
        protected ChartWindowBase(float windowWidth, float windowHeight,
            RenderTarget target) : base(windowWidth, windowHeight)
        {
            SetPadding(0.1f);
            SetBrushes(target);
            _chartColor = Color.AliceBlue;
        }
       

        #region Methods
        
        protected void SetPadding(float padding)
        {
            _paddings = new Paddings(padding, padding, padding, padding);
            SetChartSize();
        }

        protected void SetChartSize()
        {
            ChartWidth = WindowWidth * (_paddings.PaddingRightRatio - _paddings.PaddingLeftRatio);
            ChartHeight = WindowHeight * (_paddings.PaddingBottomRatio - _paddings.PaddingTopRatio);
        }

        public void ChangeColor()
        {
            if (_chartColor == Color.AliceBlue)
            {
                _chartColor = Color.DarkCyan;
                return;
            }
            _chartColor = Color.AliceBlue;
        }

        public void SetVectors()
        {
            AxeSetting = new AxeSetting(WindowSize, _paddings);
        }

        public void SetBrushes(RenderTarget renderTarget)
        {
            Brushes = new Brushes(renderTarget);
        }

        public Vector2[] XAxeVectors()
        {
            return new[] { AxeSetting.XLeftPoint, AxeSetting.TouchMiddlePoint };
        }

        public Vector2[] YAxeVectors()
        {
            return new[] { AxeSetting.YTopPoint, AxeSetting.TouchMiddlePoint };
        }
  
        #endregion

    }
}
