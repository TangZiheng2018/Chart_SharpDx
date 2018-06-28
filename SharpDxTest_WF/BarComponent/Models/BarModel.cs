using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDxTest_WF.BarComponent.Models;
using SharpDX.Mathematics.Interop;

namespace SharpDxTest_WF.BarComponent
{
    public class BarModel
    {
        #region Fields

        private double _high;
        private double _low;
        private double _open;
        private double _close;
        private bool _isBear;
        private DateTime _time;

        #endregion

        #region Properties

        public double High
        {
            get
            {
                return _high;
            }

            set
            {
                if (value > 0)
                    _high = value;
            }
        }
        public double Low
        {
            get
            {
                return _low;
            }

            set
            {
                if (value > 0)
                    _low = value;
            }
        }
        public double Open
        {
            get
            {
                return _open;
            }

            set
            {
                if (value > 0)
                    _open = value;
            }
        }
        public double Close
        {
            get
            {
                return _close;
            }

            set
            {
                if (value > 0)
                    _close = value;
            }
        }
        public DateTime Time
        {
            get
            {
                return _time;
            }

            set
            {
                if (value > DateTimeOffset.MinValue)
                    _time = value;
            }
        }
        public bool IsBear
        {
            get => _isBear;
            set => _isBear = value;
        }

        #endregion

        #region Methods

        public DateTime ToCorectDateTime(TimingBy timeBy, int timeInMinutes)
        {
            var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            var timeSpan = TimeSpan.FromSeconds(timeInMinutes);
            var dateTimeForPlacing = new DateTime();

            switch (timeBy)
            {
                case TimingBy.Minute:
                {
                    dateTimeForPlacing = origin.AddMinutes(timeSpan.TotalDays);
                    break;
                }
                case TimingBy.Hour:
                {
                    dateTimeForPlacing = origin.AddHours(timeSpan.TotalDays);
                    break;
                }
                case TimingBy.Day:
                {
                    dateTimeForPlacing = origin.AddDays(timeSpan.TotalDays);
                    break;
                }
            }

            return dateTimeForPlacing;
        }

        #endregion
    }
}
