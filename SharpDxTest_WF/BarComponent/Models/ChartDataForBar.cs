using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpDxTest_WF.BarComponent.Models
{
    public class ChartDataForBar
    {
        public float Width { get; set; }

        public float Height { get; set; }

        public float BarWidth { get; set; }

        public float SpaceBetweenBars { get; set; }

        public float TimePerPixel { get; set; }

        public float CountBarsPerChart { get; set; }

        public TimingBy TimeBy { get; set; }
    }
}
