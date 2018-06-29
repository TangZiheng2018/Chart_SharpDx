using System;
using SharpDxTest_WF.BarComponent.Models;

namespace SharpDxTest_WF.ChartRendering.Helpers
{
    public class ChartAxesMinMaxValues
    {

        #region Fields

        private BarMinMaxPositions _positions;

        private float _minValueLocation;
        private float _maxValueLocation;
        private DateTime _minDateLocation;
        private DateTime _maxDateLocation;

        #endregion

        #region Constructor

        public ChartAxesMinMaxValues(BarMinMaxPositions positions)
        {
            _positions = positions;
            MaxValueLocation = positions.MaxValue;
            MinValueLocation = positions.MinValue;
            MaxDateLocation = positions.MaxDate;
            MinDateLocation = positions.MinDate;
        }

        #endregion

        #region Properties

        public BarMinMaxPositions Positions
        {
            get => _positions;
            set => _positions = value ?? throw new NullReferenceException();
        }

        //variable which we could change while resizing or zooming and we can save origin max/min vars
        public float MinValueLocation
        {
            get => _minValueLocation;
            set => _minValueLocation = Math.Abs(value) > 0.0f ? value : throw new NullReferenceException();
        }

        public float MaxValueLocation
        {
            get => _maxValueLocation;
            set => _maxValueLocation = Math.Abs(value) > 0.0f ? value : throw new NullReferenceException();
        }

        public DateTime MinDateLocation
        {
            get => _minDateLocation;
            set => _minDateLocation = value > DateTime.MinValue ? value : throw new NullReferenceException();
        }

        public DateTime MaxDateLocation
        {
            get => _maxDateLocation;
            set => _maxDateLocation = value > DateTime.MinValue ? value : throw new NullReferenceException();
        }

        #endregion

        public float GetTimeDifference(TimingBy timing)
        {
            var time = 0.0;

            switch (timing)
            {
                case TimingBy.Minute:
                {
                    time = MaxDateLocation.Subtract(MinDateLocation).TotalMinutes;
                }
                break;

                case TimingBy.Hour:
                {
                    time = MaxDateLocation.Subtract(MinDateLocation).TotalHours;
                }
                break;

                case TimingBy.Day:
                {
                    time = MaxDateLocation.Subtract(MinDateLocation).TotalDays;
                }
                break;

                default: throw new ArgumentOutOfRangeException();
            }
            return Convert.ToSingle(time);
        }

    }
}
