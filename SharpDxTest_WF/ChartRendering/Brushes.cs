using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct2D1;

namespace SharpDxTest_WF.ChartRendering
{
    public class Brushes
    {
        public SolidColorBrush Red { get; }
        public SolidColorBrush Green { get; }
        public SolidColorBrush Black { get; }
        public SolidColorBrush TransparentBlack { get; }

        public Brushes(RenderTarget renderTarget)
        {
            Red = new SolidColorBrush(renderTarget, Color.Red);
            Green = new SolidColorBrush(renderTarget, Color.LimeGreen);
            Black = new SolidColorBrush(renderTarget, Color.Black);
            TransparentBlack = new SolidColorBrush(renderTarget, Color.Black, new BrushProperties { Opacity = 0.25f });
        }

    }
}
