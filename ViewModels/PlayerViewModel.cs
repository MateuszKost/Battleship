using MainObjects;

namespace ViewModels
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
