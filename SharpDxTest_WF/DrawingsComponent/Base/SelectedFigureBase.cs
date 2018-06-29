using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDxTest_WF.Drawings;
using SharpDxTest_WF.HelperModels;
using SharpDX.Direct2D1;

namespace SharpDxTest_WF.DrawingsComponent.Base
{
    public abstract class SelectedFigureBase : DrawingBase
    {
        protected SelectedFigureBase(RenderTarget render) : base(render)
        {

        }

        public abstract bool SetPosition(ScreenPoint point);

        public abstract bool IsFigureCrossed(ScreenPoint point);

        public abstract bool ReplaceFigure(ScreenPoint point);

        public abstract void RenderSelectedFigure();
    }
}
