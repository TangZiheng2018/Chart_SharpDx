using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.Direct2D1;

namespace SharpDxTest_WF.Models
{
    public abstract class RenderingBase
    {
        private RenderTarget _render;

        protected RenderingBase(RenderTarget renderTarget)
        {
            _render = renderTarget;
        }

        public RenderTarget Render
        {
            get => _render;
            set => _render = value != null ? value : throw new NullReferenceException();
        }

        public abstract void StartRendering();
    }
}
