using CommonObjects;

namespace MainObjects
{
    /*
     * ExtraPoint - struct for storing information about single point and its  status
     * 
     * Point - ifnormations about it coordinates
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

        public static ExtraPoint CreateExtraPoint(Point point, PointStatus pointStatus = PointStatus.Free)
        {
            return new ExtraPoint(point, pointStatus);
        }
    }
}
