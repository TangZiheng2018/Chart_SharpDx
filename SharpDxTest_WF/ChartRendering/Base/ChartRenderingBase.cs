using SharpDX.Direct2D1;

namespace SharpDxTest_WF.ChartRendering.Base
{
    public abstract class ChartRenderingBase
    {
        private ChartDrawing _chartDrawing;
        private RenderTarget _render;

        protected ChartRenderingBase(ChartDrawing chartDrawing)
        {
            ChartDrawing = chartDrawing;
        }

        public ChartDrawing ChartDrawing
        {
            get => _chartDrawing; set
            {
                _chartDrawing = value;
            }
        }
        public RenderTarget Render { get => _render; set => _render = value; }

        public abstract void StartRendering();
    }
}
