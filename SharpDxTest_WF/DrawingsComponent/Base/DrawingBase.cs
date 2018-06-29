using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDxTest_WF.DrawingsComponent.AdditionalModels;
using SharpDxTest_WF.HelperModels;
using SharpDxTest_WF.Models;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;

namespace SharpDxTest_WF.DrawingsComponent.Base
{
    public abstract class DrawingBase : RenderingBase
    {
        public SolidColorBrush BorderBrush
        {
            get;
        }

        public SolidColorBrush BackgroundBrush
        {
            get;
        }

        protected DrawingBase(RenderTarget render) : base(render)
        {
            BorderBrush = new SolidColorBrush(Render, Color.Black);
            BackgroundBrush = new SolidColorBrush(Render, Color.Blue,new BrushProperties { Opacity = 0.2f});
        }
        
        public abstract void RenderPreview(ScreenPoint point);
    }
}
