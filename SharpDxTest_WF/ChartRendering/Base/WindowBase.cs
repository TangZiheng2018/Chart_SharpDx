using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace SharpDxTest_WF.ChartRendering.Base
{
    public abstract class WindowBase
    {
        #region Fields

        private float _windowWidth;
        private float _windowHeight;

        #endregion

        #region Properties

        public float WindowWidth
        {
            get => _windowWidth;
            protected set => _windowWidth = value;
        }

        public float WindowHeight
        {
            get => _windowHeight;
            protected set => _windowHeight = value;
        }

        public Size2F GetWindowSize
        {
            get => new Size2F(WindowWidth, WindowHeight);
        }

        #endregion

        #region Constructors
        protected WindowBase(float windowWidth, float windowHeight)
        {
            WindowWidth = windowWidth;
            WindowHeight = windowHeight;
        }

        protected WindowBase(Size windowSize)
        {
            WindowWidth = windowSize.Width;
            WindowHeight = windowSize.Height;
        }

        #endregion

    }

}
