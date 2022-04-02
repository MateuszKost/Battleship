using MainObjects;

namespace BattlehipGame.Server.ViewModels
{
    public class ShootViewModel
    {
        public ExtraPoint Point { get;}
        public bool Turn { get; }

        public ShootViewModel(ExtraPoint point, bool turn)
        {
            Point = point;
            Turn = turn;
        }
    }
}
