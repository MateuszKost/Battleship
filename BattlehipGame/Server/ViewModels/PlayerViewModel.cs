using MainObjects;

namespace BattlehipGame.Server.ViewModels
{
    public class PlayerViewModel
    {
        public string PlayerName { get;}
        public Point[] Map { get; }

        public PlayerViewModel(string playerName, Point[] map)
        {
            PlayerName = playerName;
            Map = map;
        }
    }
}
