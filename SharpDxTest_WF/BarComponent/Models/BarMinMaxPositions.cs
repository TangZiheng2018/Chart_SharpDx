using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpDxTest_WF.BarComponent.Models
{
    public class BarMinMaxPositions
    {

        public float MinValue { get; set; }  
        public float MaxValue { get; set; }  
        public DateTime MinDate { get; set; }  
        public DateTime MaxDate { get; set; }
        

        public float GetTimeDifference(TimingBy timing)
        {
            double time = 0;

            switch (timing)
            {
                case TimingBy.Minute:
                {
                    time = MaxDate.Subtract(MinDate).TotalMinutes;
                }
                break;

                case TimingBy.Hour:
                {
                    time = MaxDate.Subtract(MinDate).TotalHours;
                }
                break;

                case TimingBy.Day:
                {
                    time = MaxDate.Subtract(MinDate).TotalDays;
                }
                break;

                default: throw new ArgumentOutOfRangeException();

            }
            return Convert.ToSingle(time);
        }
    }
}
