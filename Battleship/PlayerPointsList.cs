using CommonObjects;
using MainObjects;
using System.Collections.Generic;

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

        internal void CheckPointStatusForPlayer(PlayerPointsDictionary playerNextProbablyPoints, PointStatus pointStatus, Point _lastPoint)
        {
            switch (pointStatus)
            {
                case PointStatus.Missed:
                    this.Add(_lastPoint);
                    if (playerNextProbablyPoints.Any())
                    {
                        playerNextProbablyPoints.CheckDictionariesForPlayer(_lastPoint);
                    }
                    break;
                case PointStatus.Hit:
                    this.Add(_lastPoint);
                    playerNextProbablyPoints.CheckDictionariesForPlayer(_lastPoint);
                    break;
                case PointStatus.Sunk:
                    this.Add(_lastPoint);
                    playerNextProbablyPoints.Clear();
                    break;
            }
        }

    }
}
