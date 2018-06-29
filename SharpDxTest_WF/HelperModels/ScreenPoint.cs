using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpDxTest_WF.HelperModels
{
    public class ScreenPoint
    {
        public float X { get; set; }

        public float Y { get; set; }

        public ScreenPoint()
        {
            
        }

        public ScreenPoint(float x, float y)
        {
            X = x;
            Y = y;
        }

        public ScreenPoint(double x, double y)
        {
            X = (float)x;
            Y = (float)y;
        }
    }
}
