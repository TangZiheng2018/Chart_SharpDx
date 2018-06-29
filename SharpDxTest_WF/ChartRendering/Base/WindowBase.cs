using System;
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
            protected set => _windowWidth = Math.Abs(value) > 0.0f ? value : throw new NullReferenceException();
        }

        public float WindowHeight
        {
            get => _windowHeight;
            protected set => _windowHeight = Math.Abs(value) > 0.0f ? value : throw new NullReferenceException();
        }

        public Size2F WindowSize => new Size2F(WindowWidth, WindowHeight);

        #endregion

        #region Constructors
        protected WindowBase(float windowWidth, float windowHeight)
        {
            WindowWidth = windowWidth;
            WindowHeight = windowHeight;
        }

        #endregion

    }

}
