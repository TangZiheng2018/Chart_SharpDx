namespace SharpDxTest_WF.HelperModels
{
    public struct ScreenPoint
    {
        public float X { get; set; }

        public float Y { get; set; }
        
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
