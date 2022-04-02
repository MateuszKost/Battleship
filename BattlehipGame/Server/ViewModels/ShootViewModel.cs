using MainObjects;

namespace BattlehipGame.Server.ViewModels
{
    public class ShootViewModel
    {
        public Point Point { get;}
        public bool Turn { get; }

        public ShootViewModel(Point point, bool turn)
        {
            Point = point;
            Turn = turn;
        }
    }
}
