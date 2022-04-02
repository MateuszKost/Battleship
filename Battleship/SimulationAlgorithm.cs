using CommonObjects;
using MainObjects;

namespace Battleship
{
    internal class SimulationAlgorithm
    {
        private readonly Random _random = new Random();
        private readonly PlayerPointsList _firstPlayerPointsToShoot;
        private readonly PlayerPointsList _firstPlayerLastHitPoints;
        private readonly PlayerPointsList _secondPlayerPointsToShoot;
        private readonly PlayerPointsList _secondPlayerLastHitPoints;
        private readonly PlayerPointsDictionary _firstPlayerNextProbablyPoints;
        private readonly PlayerPointsDictionary _secondPlayerNextProbablyPoints;
        private Point _lastPoint = Point.CreatePoint(int.MinValue, char.MinValue);

        public SimulationAlgorithm()
        {
            _firstPlayerPointsToShoot = new PlayerPointsList();
            _firstPlayerLastHitPoints = new PlayerPointsList();
            _secondPlayerPointsToShoot = new PlayerPointsList();
            _secondPlayerLastHitPoints = new PlayerPointsList();
            _firstPlayerNextProbablyPoints = new PlayerPointsDictionary();
            _secondPlayerNextProbablyPoints = new PlayerPointsDictionary();
        }

        internal void Start(Player playerOne, Player playerTwo)
        {
            PointStatus pointStatus;
            bool playerTurn;
            FillDictionaries();

            if (_random.Next(CommonVariables.Players.Count()) == CommonVariables.Zero)
            {
                playerTurn = CommonVariables.SecondPlayer;
                Console.WriteLine(CommonVariables.FirstPlayerStartsTheGame);
            }
            else
            {
                playerTurn = CommonVariables.FirstPlayer;
                Console.WriteLine(CommonVariables.SecondPlayerStartsTheGame);
            }

            pointStatus = ShootAndCheckIfHit(playerOne, playerTwo, playerTurn);

            while (playerOne.Ships.Count != CommonVariables.Zero && playerTwo.Ships.Count != CommonVariables.Zero)
            {
                if (pointStatus == PointStatus.Missed)
                {
                    playerTurn = !playerTurn;
                    if (playerTurn)
                        Console.WriteLine(CommonVariables.FirstPlyaerTurn);
                    else
                        Console.WriteLine(CommonVariables.SecondPlayerTurn);
                }
                pointStatus = ShootAndCheckIfHit(playerOne, playerTwo, playerTurn);
            }
        }

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
                _firstPlayerLastHitPoints.CheckPointStatusForPlayer(_firstPlayerNextProbablyPoints, pointStatus, _lastPoint);
            }
            else
            {
                pointStatus = SecondPlayerShoot(playerOne, playerTwo);
                pointStatus = CheckIfShotHitForSecondPlayer(playerOne, playerTwo, pointStatus);
                _secondPlayerLastHitPoints.CheckPointStatusForPlayer(_secondPlayerNextProbablyPoints, pointStatus, _lastPoint);
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
    }
}
