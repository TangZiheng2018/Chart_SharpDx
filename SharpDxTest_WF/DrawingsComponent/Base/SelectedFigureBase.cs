using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDxTest_WF.Drawings;
using SharpDX.Direct2D1;

namespace SharpDxTest_WF.DrawingsComponent.Base
{
    public abstract class SelectedFigureBase : DrawingBase
    {
        public bool IsSelected { get; protected set; }

        protected SelectedFigureBase(RenderTarget render) : base(render)
        {
        }


        public abstract bool SetPosition(float x, float y);

        public abstract bool IsFigureCrossed(float x, float y);

        public abstract bool ReplaceFigure(float x, float y);

        public abstract void RenderSelectedFigure();
    }
}
