using CommonObjects;
using MainObjects;
using ViewModels;

namespace Battleship
{
    internal class PlayerPointsList : List<Point>
    {
        private Random _random = new Random();

        internal Point CreateNewPointForPlayer(PlayerPointsList playerPointsToShoot)
        {
            Point result = playerPointsToShoot.ElementAt(_random.Next(playerPointsToShoot.Count));
            while (this.Contains(result))
            {
                result = playerPointsToShoot.ElementAt(_random.Next(playerPointsToShoot.Count));
            }
            playerPointsToShoot.Remove(result);
            return result;
        }

        internal void CheckPointStatusForPlayer(PlayerPointsDictionary playerNextProbablyPoints, PlayerPointsDictionary playerDeletedPoints, ICollection<ShootViewModel> shootForApi, PointStatus pointStatus, Point _lastPoint, bool playerTurn)
        {
            switch (pointStatus)
            {
                case PointStatus.Missed:
                    if (playerNextProbablyPoints.Any())
                    {
                        playerNextProbablyPoints.CheckDictionariesForPlayer(_lastPoint, pointStatus, playerDeletedPoints);
                    }
                    shootForApi.Add(ShootViewModel.CreateShootViewModel(ExtraPoint.CreateExtraPoint(_lastPoint, pointStatus), playerTurn));
                    break;
                case PointStatus.Hit:
                    playerNextProbablyPoints.CheckDictionariesForPlayer(_lastPoint, pointStatus, playerDeletedPoints);
                    shootForApi.Add(ShootViewModel.CreateShootViewModel(ExtraPoint.CreateExtraPoint(_lastPoint, pointStatus), playerTurn));
                    break;
                case PointStatus.HitButNoSunk:
                    shootForApi.Add(ShootViewModel.CreateShootViewModel(ExtraPoint.CreateExtraPoint(_lastPoint, PointStatus.Hit), playerTurn));
                    playerNextProbablyPoints.ReturnDeletedPoints(playerDeletedPoints);
                    break;
                case PointStatus.Sunk:
                    playerNextProbablyPoints.Clear();
                    shootForApi.Add(ShootViewModel.CreateShootViewModel(ExtraPoint.CreateExtraPoint(_lastPoint, PointStatus.Hit), playerTurn));
                    break;
            }
            this.Add(_lastPoint);
        }
    }
}
