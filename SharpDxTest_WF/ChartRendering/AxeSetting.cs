using System;
using SharpDxTest_WF.ChartRendering.Helpers;
using SharpDX;

namespace SharpDxTest_WF.ChartRendering
{
    public class AxeSetting
    {
        #region Fields

        private Vector2 _touchMiddlePoint;
        private Vector2 _xLeftPoint;
        private Vector2 _yTopPoint;

        #endregion

        #region Properties

        public float PointOnEveryPercentAxeX { get; }
        public float PointOnEveryPercentAxeY { get; }

        public float CountPointsOnYAxe { get; }
        public float CountPointsOnXAxe { get; }

        public float YValuePointStart { get; private set; }
        public float YValuePointFinish { get; private set; }

        public float XDatePointStart { get; private set; }
        public float XDatePointFinish { get; private set; }

        public Vector2 TouchMiddlePoint
        {
            get => _touchMiddlePoint;
            private set => _touchMiddlePoint = value != Vector2.Zero ? value : throw new NullReferenceException();
        }

        public Vector2 XLeftPoint
        {
            get => _xLeftPoint;
            private set => _xLeftPoint = value != Vector2.Zero ? value : throw new NullReferenceException();
        }

        public Vector2 YTopPoint
        {
            get => _yTopPoint;
            private set => _yTopPoint = value != Vector2.Zero ? value : throw new NullReferenceException();
        }


        #endregion

        #region Constructors

        public AxeSetting(Size2F windoSize, Paddings paddings)
        {
            SetAxeVectors(windoSize.Height, windoSize.Width, paddings);

            SetPointsBorders(windoSize.Height, windoSize.Width, paddings);

            PointOnEveryPercentAxeY = 0.05f;
            PointOnEveryPercentAxeX = 0.1f;

            var chartHeight = windoSize.Height * (paddings.PaddingBottomRatio - paddings.PaddingTopRatio);
            var chartWidth = windoSize.Width * (paddings.PaddingRightRatio - paddings.PaddingLeftRatio);

            CountPointsOnYAxe = chartHeight / (chartHeight * PointOnEveryPercentAxeY);
            CountPointsOnXAxe = chartWidth / (chartWidth * PointOnEveryPercentAxeX);
        }

        #endregion

        #region Methods

        public void SetAxeVectors(float windowHeight, float windowWidth, Paddings paddings)
        {
            TouchMiddlePoint = new Vector2(windowWidth * paddings.PaddingRightRatio, windowHeight * paddings.PaddingBottomRatio);
            XLeftPoint = new Vector2(windowWidth * (paddings.PaddingLeftRatio - 0.05f), windowHeight * paddings.PaddingBottomRatio);
            YTopPoint = new Vector2(windowWidth * paddings.PaddingRightRatio, windowHeight * (paddings.PaddingTopRatio - 0.05f));
        }

        public void SetPointsBorders(float windowHeight, float windowWidth, Paddings paddings)
        {
            YValuePointStart = windowWidth * paddings.PaddingRightRatio;
            YValuePointFinish = windowWidth * (paddings.PaddingRightRatio + 0.02f);

            XDatePointStart = windowHeight * paddings.PaddingBottomRatio;
            XDatePointFinish = windowHeight * (paddings.PaddingBottomRatio + 0.02f);
        }

        #endregion

    }
}
