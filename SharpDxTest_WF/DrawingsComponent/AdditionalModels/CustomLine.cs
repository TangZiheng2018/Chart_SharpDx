using SharpDX;

namespace SharpDxTest_WF.DrawingsComponent.AdditionalModels
{
    public class CustomLine
    {
        public Vector2 Point1 { get; set; }
        public Vector2 Point2 { get; set; }

        public CustomLine()
        {
            Point1 = Vector2.Zero;
            Point2 = Vector2.Zero;
        }

        public CustomLine(Vector2 vector)
        {
            SetVector(vector);
        }

        public CustomLine(Vector2 v1, Vector2 v2)
        {
            Point1 = v1;
            Point2 = v2;
        }


        public bool SetVector(Vector2 vector)
        {
            if (Point1 == Vector2.Zero)
            {
                Point1 = vector;
                return false;
            }

            Point2 = vector;
            return true;
        }

        public void ClearVectors()
        {
            Point1 = Vector2.Zero;
            Point2 = Vector2.Zero;
        }

    }

}
