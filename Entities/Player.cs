namespace Entities
{
    /*
     * Player - class for storing information about player
     * 
     * NickName - name of the player
     * Ships - the list of ships assigned to the player
     */
    public class Player
    {
        public string NickName { get; set; }
        public List<Ship> Ships { get; set; }

        private Player(string nickName, List<Ship> ships)
        {
            NickName = nickName;
            Ships = ships;
        }

        public static Player CreatePlayer(string nickName, List<Ship> ships)
        {
            return new Player(nickName, ships);
        }
    }
}
