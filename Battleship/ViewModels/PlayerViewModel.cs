using MainObjects;

namespace Battlehip.ViewModels
{
    public class PlayerViewModel
    {
        public string PlayerName { get;}
        public ExtraPoint[] Map { get; }

        public PlayerViewModel(string playerName, ExtraPoint[] map)
        {
            PlayerName = playerName;
            Map = map;
        }
    }
}
