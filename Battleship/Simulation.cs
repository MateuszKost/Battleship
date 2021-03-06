using CommonObjects;
using MainObjects;
using ViewModels;

namespace Battleship
{
    public class Simulation
    {
        private readonly Random _random = new Random();
        private readonly PlayerPointsList _firstPlayerPointsToShoot;
        private readonly PlayerPointsList _firstPlayerLastHitPoints;
        private readonly PlayerPointsList _secondPlayerPointsToShoot;
        private readonly PlayerPointsList _secondPlayerLastHitPoints;
        private readonly PlayerPointsDictionary _firstPlayerNextProbablyPoints;
        private readonly PlayerPointsDictionary _firstPlayerDeletedPoints;
        private readonly PlayerPointsDictionary _secondPlayerNextProbablyPoints;
        private readonly PlayerPointsDictionary _secondPlayerDeletedPoints;
        private readonly ICollection<ShootViewModel> _shootForApi;
        private Point _lastPoint = Point.CreatePoint(int.MinValue, char.MinValue);

        public Simulation()
        {
            _firstPlayerPointsToShoot = new PlayerPointsList();
            _firstPlayerLastHitPoints = new PlayerPointsList();
            _secondPlayerPointsToShoot = new PlayerPointsList();
            _secondPlayerLastHitPoints = new PlayerPointsList();
            _firstPlayerNextProbablyPoints = new PlayerPointsDictionary();
            _firstPlayerDeletedPoints = new PlayerPointsDictionary();
            _secondPlayerNextProbablyPoints = new PlayerPointsDictionary();
            _secondPlayerDeletedPoints = new PlayerPointsDictionary();
            _shootForApi = new List<ShootViewModel>();
        }

        public ICollection<ShootViewModel> Start(Player playerOne, Player playerTwo)
        {
            if (playerOne == null || playerTwo == null)
            {
                throw new ArgumentNullException(nameof(Player));
            }

            PointStatus pointStatus;
            bool playerTurn;
            FillDictionaries();

            if (_random.Next(CommonVariables.Players.Count()) == CommonVariables.Zero)
            {
                playerTurn = CommonVariables.SecondPlayer;
                playerOne.StartsGame();
            }
            else
            {
                playerTurn = CommonVariables.FirstPlayer;
                playerTwo.StartsGame();
            }

            pointStatus = ShootAndCheckIfHit(playerOne, playerTwo, playerTurn);

            while (playerOne.Ships.Count != CommonVariables.Zero && playerTwo.Ships.Count != CommonVariables.Zero)
            {
                if (pointStatus == PointStatus.Missed)
                {
                    playerTurn = !playerTurn;
                    if (playerTurn)
                    {
                        playerOne.Turn();
                    }
                    else
                    {
                        playerTwo.Turn();
                    }
                }
                pointStatus = ShootAndCheckIfHit(playerOne, playerTwo, playerTurn);
            }
            return _shootForApi;
        }

        #region private functions
        private void FillDictionaries()
        {
            foreach (char y in CommonVariables.DefaultYAxis)
            {
                foreach (int x in CommonVariables.DefaultXAxis)
                {
                    _firstPlayerPointsToShoot.Add(Point.CreatePoint(x, y));
                    _secondPlayerPointsToShoot.Add(Point.CreatePoint(x, y));
                }
            }
        }

        private PointStatus ShootAndCheckIfHit(Player playerOne, Player playerTwo, bool playerTurn)
        {
            PointStatus pointStatus;
            if (playerTurn)
            {
                pointStatus = FirstPlayerShoot(playerOne, playerTwo);
                pointStatus = CheckIfShotHitForFirstPlayer(playerOne, playerTwo, pointStatus);
                _firstPlayerLastHitPoints.CheckPointStatusForPlayer(_firstPlayerNextProbablyPoints, _firstPlayerDeletedPoints, _shootForApi, pointStatus, _lastPoint, playerTurn);
            }
            else
            {
                pointStatus = SecondPlayerShoot(playerOne, playerTwo);
                pointStatus = CheckIfShotHitForSecondPlayer(playerOne, playerTwo, pointStatus);
                _secondPlayerLastHitPoints.CheckPointStatusForPlayer(_secondPlayerNextProbablyPoints, _secondPlayerDeletedPoints, _shootForApi, pointStatus, _lastPoint, playerTurn);
            }

            return pointStatus;
        }

        private PointStatus FirstPlayerShoot(Player playerOne, Player playerTwo)
        {
            Point point = _firstPlayerNextProbablyPoints.CreatePointForPlayer(_firstPlayerLastHitPoints, _firstPlayerPointsToShoot);
            _lastPoint = point;
            return playerOne.Shoot(point, playerTwo.Ships);
        }

        private PointStatus SecondPlayerShoot(Player playerOne, Player playerTwo)
        {
            Point point = _secondPlayerNextProbablyPoints.CreatePointForPlayer(_secondPlayerLastHitPoints, _secondPlayerPointsToShoot);
            _lastPoint = point;
            return playerTwo.Shoot(point, playerOne.Ships);
        }

        private PointStatus CheckIfShotHitForFirstPlayer(Player playerOne, Player playerTwo, PointStatus pointStatus)
        {
            switch (pointStatus)
            {
                case PointStatus.Missed:
                    playerOne.UpdateEnemyMap(_lastPoint, pointStatus);
                    return pointStatus;
                case PointStatus.Hit:
                    playerOne.UpdateEnemyMap(_lastPoint, pointStatus);
                    return playerTwo.UpdateOwnMap(_lastPoint);
            }
            return pointStatus;
        }

        private PointStatus CheckIfShotHitForSecondPlayer(Player playerOne, Player playerTwo, PointStatus pointStatus)
        {
            switch (pointStatus)
            {
                case PointStatus.Missed:
                    playerTwo.UpdateEnemyMap(_lastPoint, pointStatus);
                    return pointStatus;
                case PointStatus.Hit:
                    playerTwo.UpdateEnemyMap(_lastPoint, pointStatus);
                    return playerOne.UpdateOwnMap(_lastPoint);
            }
            return pointStatus;
        }
        #endregion
    }
}
