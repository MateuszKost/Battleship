namespace CommonObjects
{
    public static class CommonVariables
    {
        public const bool FirstPlayer = true;
        public const bool SecondPlayer = false;
        public const int DefaultMapSize = 100;
        public const int FirstIndexOfX_Y_Axis = 0;
        public const int LastIndexOfX_Y_Axis = 9;
        public const int Horizontal = 0, Previous = 0, Zero = 0;
        public const int Vertical = 1, Next = 1;
        public const string Hit = "Trafiono";
        public const string Missed = "Pudlo";
        public const string ShotPointInPlace = "Oddano strzał w {0}{1}";
        public const string ShipWithNumberSunk = "{0} gracza {1} został zatopiony";
        public const string PlayerWithNumberLost = "Gracz {0} przegrał gre";

        public static readonly int[] DefaultXAxis = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        public static readonly char[] DefaultYAxis = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j' };
        public static readonly IEnumerable<int> Arrangement = new List<int> { Horizontal, Vertical };
        public static readonly IEnumerable<int> Order = new List<int> { Previous, Next };
        public static readonly IEnumerable<bool> Players = new List<bool> { FirstPlayer, SecondPlayer };
    }
}
