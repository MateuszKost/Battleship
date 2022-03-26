namespace MainObjects
{
    /*
     * Player - class for storing information about player
     * 
     * NickName - name of the player
     * Ships - the list of ships assigned to the player
     * OwnMap - Array with points which is representing own board like in board game
     * EnemyMap - Array with points which is representing enemy board like in board game
     */
    public class Player
    {
        public string NickName { get; set; }
        public List<Ship> Ships { get; set; }
        public Point[] OwnMap { get; set; }
        public Point[] EnemyMap { get; set; }


        private Player(string nickName, List<Ship> ships, int mapSize)
        {
            NickName = nickName;
            Ships = ships;
            OwnMap = new Point[mapSize];
            EnemyMap = new Point[mapSize];
        }

        public static Player CreatePlayer(string nickName, List<Ship>? ships = null, int mapSize = CommonObjects.CommonVariables.DefaultMapSize)
        {
            if(ships == null)
            {
                ships = new List<Ship>();
            }

            return new Player(nickName, ships, mapSize);
        }
    }
}
