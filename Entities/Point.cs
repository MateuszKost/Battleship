namespace MainObjects
{
    public struct Point
    {
        /**
         * Point - struct for storing information about coorinates of single point
         * 
         * X - like in coordinate system x value
         * Y - like in coordinate system y value 
         */

        public int X { get; set; } // changed from only getter to get; set; in case of problems with deserialization
        public char Y { get; set; } // changed from only getter to get; set; in case of problems with deserialization

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
