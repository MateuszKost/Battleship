using CommonObjects;

namespace MainObjects
{
    /*
     * Point - class for storing information about single point of ship length
     * 
     * X - like in coordinate system x value
     * Y - like in coordinate system y value
     * Status - informing us about point status
     */
    public class Point
    {
        public int X { get; private set; }
        public char Y { get; private set; }
        public PointStatus Status { get; set; }

        private Point(int x, char y)
        {
            X = x;
            Y = y;
            Status = PointStatus.Free;
        }

        public static Point CreatePoint(int x, char y)
        {
            return new Point(x, y);
        }
    }
}
