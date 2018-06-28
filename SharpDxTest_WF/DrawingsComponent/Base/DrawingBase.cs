using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDxTest_WF.DrawingsComponent.AdditionalModels;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;

namespace SharpDxTest_WF.DrawingsComponent.Base
{
    public abstract class DrawingBase
    {
        protected RenderTarget _render;

        protected SolidColorBrush _borderBrush;
        protected SolidColorBrush _backgroundBrush;

        protected DrawingBase(RenderTarget render)
        {
            _render = render;
            _borderBrush = new SolidColorBrush(_render, Color.Black);
            _backgroundBrush = new SolidColorBrush(_render, Color.Blue,new BrushProperties { Opacity = 0.2f});
        }
        
        public abstract void RenderFigure();
        public abstract void RenderPreview(float dx, float dy);
    }
}
