using MainObjects;

namespace Battlehip.ViewModels
{
    public class ShootViewModel
    {
        public ExtraPoint Point { get;}
        public bool Turn { get; }

        private  ShootViewModel(ExtraPoint point, bool turn)
        {
            Point = point;
            Turn = turn;
        }

        public static ShootViewModel CreateShootViewModel(ExtraPoint point, bool turn)
        {
            return new ShootViewModel(point, turn);
        }
    }
}
