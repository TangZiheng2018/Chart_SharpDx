using System;

namespace SharpDxTest_WF.ChartRendering.Helpers
{
    public class Paddings
    {

        #region Fields

        private float _paddingLeftRatio;
        private float _paddingRightRatio;
        private float _paddingTopRatio;
        private float _paddingBottomRatio;

        #endregion

        #region Properties

        public float PaddingTopRatio
        {
            get => _paddingTopRatio;
            set => _paddingTopRatio = Math.Abs(value) > 0 ? value : throw new NullReferenceException();
        }

        public float PaddingBottomRatio
        {
            get => _paddingBottomRatio;
            set => _paddingBottomRatio = Math.Abs(value) > 0 ? value : throw new NullReferenceException();
        }

        public float PaddingRightRatio
        {
            get => _paddingRightRatio;
            set => _paddingRightRatio = Math.Abs(value) > 0 ? value : throw new NullReferenceException();
        }

        public float PaddingLeftRatio
        {
            get => _paddingLeftRatio;
            set => _paddingLeftRatio = Math.Abs(value) > 0 ? value : throw new NullReferenceException();
        }

        #endregion

        #region Constructors
        
        public Paddings(float left, float top, float right, float bottom)
        {
            PaddingLeftRatio = left;
            PaddingTopRatio = top;
            PaddingRightRatio = 1 - right;
            PaddingBottomRatio = 1 - bottom;
        }


        #endregion

    }
}
