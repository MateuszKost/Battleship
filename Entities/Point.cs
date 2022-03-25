namespace Entities
{
    /*
     * Point - class for storing information about single point of ship length
     * 
     * X - like in coordinate system x value
     * Y - like in coordinate system y value
     * Status - setting value when point which is used in ship is hitted or not
     */
    public class Point
    {
        public int X { get; private set; }
        public char Y { get; private set; }
        public bool Status { get; set; } = false;

        private Point(int x, char y)
        {
            X = x;
            Y = y;
        }

        public static Point CreatePoint(int x, char y)
        {
            return new Point(x, y);
        }
    }
}
