namespace CommonObjects
{
    public static class CommonVariables
    {
        public const int DefaultMapSize = 100;
        public const int FirstIndexOfX_Y_Axis = 0;
        public const int LastIndexOfX_Y_Axis = 9;

        public static readonly int Horizontal, Previous = 0;
        public static readonly int Vertical, Next = 1;

        public static readonly int[] DefaultXAxis = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        public static readonly char[] DefaultYAxis = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j' };
        public static readonly IEnumerable<int> Arrangement = new List<int> { Horizontal, Vertical };
        public static readonly IEnumerable<int> Order = new List<int> { Previous, Next };
    }
}
