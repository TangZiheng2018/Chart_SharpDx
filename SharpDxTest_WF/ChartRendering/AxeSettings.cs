using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace SharpDxTest_WF.ChartRendering.Helpers
{
    public class AxeSettings
    {
        public Vector2 touchXYPoint;
        public Vector2 xLeftPoint;
        public Vector2 yTopPoint;

        public float PointOnEveryPercentAxeX
        {
            get;
            private set;
        }
        public float PointOnEveryPercentAxeY

        {
            get;
            private set;
        }
        public float CountPointsOnYAxe
        {
            get;
            private set;
        }

        public float CountPointsOnXAxe
        {
            get;
            private set;
        }

        public float yValuePointStart;
        public float yValuePointFinish;

        public float xDatePointStart;
        public float xDatePointFinish;


        public AxeSettings(float windowHeight, float windowWidth, Paddings paddings, float valuePerPixel)
        {
            touchXYPoint = new Vector2(windowWidth * paddings.PaddingRightRatio, windowHeight * paddings.PaddingBottomRatio);
            xLeftPoint = new Vector2(windowWidth * (paddings.PaddingLeftRatio - 0.05f), windowHeight * paddings.PaddingBottomRatio);
            yTopPoint = new Vector2(windowWidth * paddings.PaddingRightRatio, windowHeight * (paddings.PaddingTopRatio - 0.05f));

            yValuePointStart = windowWidth * paddings.PaddingRightRatio;
            yValuePointFinish = windowWidth * (paddings.PaddingRightRatio + 0.02f);

            xDatePointStart = windowHeight * paddings.PaddingBottomRatio;
            xDatePointFinish = windowHeight * (paddings.PaddingBottomRatio + 0.02f);

            PointOnEveryPercentAxeY = 0.05f;
            PointOnEveryPercentAxeX = 0.1f;

            var chartHeight = windowHeight * (paddings.PaddingBottomRatio - paddings.PaddingTopRatio);
            var chartWidth = windowWidth * (paddings.PaddingRightRatio - paddings.PaddingLeftRatio);

            CountPointsOnYAxe = chartHeight / (chartHeight * PointOnEveryPercentAxeY);
            CountPointsOnXAxe = chartWidth / (chartWidth * PointOnEveryPercentAxeX);
        }

        public void CreateAxeXPoints(float valuePerPx)
        {


        }


        public void CreateAxeYPoints(float valuePerPx)
        {


        }

        public void SetPointOnEveryPercent(float percent)
        {
            PointOnEveryPercentAxeY = percent;
        }

    }
}
