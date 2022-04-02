namespace MainObjects
{
    public struct Point
    {
        public int X { get; }
        public char Y { get; }

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
