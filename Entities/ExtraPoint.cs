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
    public struct ExtraPoint
    {
        public Point Point { get; }
        public PointStatus Status { get; set; }

        private ExtraPoint(Point point, PointStatus pointStatus)
        {
            Point = point;
            Status = pointStatus;
        }

        public static ExtraPoint CreatePoint(Point point, PointStatus pointStatus = PointStatus.Free)
        {
            return new ExtraPoint(point, pointStatus);
        }
    }
}
