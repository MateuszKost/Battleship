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
        public int X { get; init; }
        public char Y { get; init; }
        public PointStatus Status { get; set; }

        private Point(int x, char y, PointStatus pointStatus)
        {
            X = x;
            Y = y;
            Status = pointStatus;
        }

        public static Point CreatePoint(int x, char y, PointStatus pointStatus = PointStatus.Free)
        {
            return new Point(x, y, pointStatus);
        }
    }
}
