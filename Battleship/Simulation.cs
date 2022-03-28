using CommonObjects;
using MainObjects;

namespace Battleship
{
    internal class Simulation
    {
        private readonly Random _random = new Random();
        private bool _lastTurn;
        private Point _lastPoint = null;
        private readonly Dictionary<Point, IndexType> _firstPlaterLastHittedPoints;
        private readonly Dictionary<Point, IndexType> _firstPlaterNextProbablyPoints;
        private readonly Dictionary<Point, IndexType> _secondPlaterLastHittedPoints;
        private readonly Dictionary<Point, IndexType> _secondPlaterNextProbablyPoints;

        public Simulation()
        {
            _firstPlaterLastHittedPoints = new Dictionary<Point, IndexType>();
            _firstPlaterNextProbablyPoints = new Dictionary<Point, IndexType>();
            _secondPlaterLastHittedPoints = new Dictionary<Point, IndexType>();
            _secondPlaterNextProbablyPoints = new Dictionary<Point, IndexType>();
        }

        internal void Start(Player playerOne, Player playerTwo)
        {
            bool playerTurn;

            if (_random.Next(CommonVariables.Players.Count()) == CommonVariables.Zero)
            {
                playerTurn = CommonVariables.SecondPlayer;
                Console.WriteLine("Gracz pierwszy rozpoczyna gre!");
            }
            else
            {
                playerTurn = CommonVariables.FirstPlayer;
                Console.WriteLine("Gracz drugi rozpoczyna gre!");
            }

            PointStatus pointStatus = PointStatus.Free;

            pointStatus = Shot(playerOne, playerTwo, playerTurn, pointStatus);

            _lastTurn = playerTurn;

            while (playerOne.Ships.Count != CommonVariables.Zero || playerTwo.Ships.Count != CommonVariables.Zero)
            {
                if (pointStatus == PointStatus.Missed)
                {
                    playerTurn = !playerTurn;
                }
                pointStatus = Shot(playerOne, playerTwo, playerTurn, pointStatus);
                pointStatus = CheckIfShotHitted(playerOne, playerTwo, pointStatus, playerTurn);
                CheckPointStatus(pointStatus, playerTurn);
                _lastTurn = playerTurn;
            }
        }

        private PointStatus Shot(Player playerOne, Player playerTwo, bool playerTurn, PointStatus pointStatus)
        {
            Point point;
            if (playerTurn)
            {
                point = WhereToShot(playerOne, playerTurn, pointStatus);
                return playerOne.Shot(point, playerTwo.Ships);
            }
            else
            {
                point = WhereToShot(playerTwo, playerTurn, pointStatus);
                return playerTwo.Shot(point, playerOne.Ships);
            }
        }

        private Point WhereToShot(Player player, bool playerTurn, PointStatus pointStatus)
        {
            Point point;
            if ((_lastPoint != null && _lastTurn == playerTurn && pointStatus != PointStatus.Sunk) || player.HittedButNotSunk)
            {
                _lastPoint = CreateNearPoint(playerTurn);
                return _lastPoint;
            }
            else
            {
                _lastPoint = CreatePoint();
                return _lastPoint;
            }
        }

        private Point CreatePoint()
        {
            int xIndex = _random.Next(CommonVariables.LastIndexOfX_Y_Axis);
            int yIndex = _random.Next(CommonVariables.LastIndexOfX_Y_Axis);

            return Point.CreatePoint(CommonVariables.DefaultXAxis[xIndex], CommonVariables.DefaultYAxis[yIndex]);
        }

        private Point CreateNearPoint(bool playerTurn)
        {
            int index;
            if (playerTurn)
            {
                 index= _random.Next(_firstPlaterNextProbablyPoints.Count);
                return _firstPlaterNextProbablyPoints.ElementAt(index).Key;
            }
            else
            {
                index = _random.Next(_secondPlaterNextProbablyPoints.Count);
                return _secondPlaterNextProbablyPoints.ElementAt(index).Key; // getting there when it has 0 elements
            }
        }

        private PointStatus CheckIfShotHitted(Player playerOne, Player playerTwo, PointStatus pointStatus, bool playerTurn)
        {
            if (playerTurn)
            {
                switch (pointStatus)
                {
                    case PointStatus.Missed:
                        playerOne.UpdateEnemyMap(_lastPoint, pointStatus);
                        return pointStatus;
                    case PointStatus.Hitted:
                        playerOne.UpdateEnemyMap(_lastPoint, pointStatus);
                        return playerTwo.UpdateOwnMap(_lastPoint);
                }
                return pointStatus;
            }
            else
            {
                switch (pointStatus)
                {
                    case PointStatus.Missed:
                        playerOne.UpdateEnemyMap(_lastPoint, pointStatus);
                        return pointStatus;
                    case PointStatus.Hitted:
                        playerOne.UpdateEnemyMap(_lastPoint, pointStatus);
                        return playerTwo.UpdateOwnMap(_lastPoint);
                }
                return pointStatus;
            }
        }

        private void CheckPointStatus(PointStatus pointStatus, bool playerTurn)
        {
            if (playerTurn)
            {
                switch (pointStatus)
                {
                    case PointStatus.Missed:
                        _firstPlaterLastHittedPoints.Add(_lastPoint, IndexType.Default);
                        if (_firstPlaterNextProbablyPoints.Any())
                        {
                            CheckDictionaries(playerTurn);
                        }
                        break;
                    case PointStatus.Hitted:
                        _firstPlaterLastHittedPoints.Add(_lastPoint, IndexType.Default);
                        CheckDictionaries(playerTurn);
                        break;
                    case PointStatus.Sunk:
                        _firstPlaterLastHittedPoints.Add(_lastPoint, IndexType.Default);
                        _firstPlaterNextProbablyPoints.Clear();
                        break;
                }
            }
            else
            {
                switch (pointStatus)
                {
                    case PointStatus.Missed:
                        _secondPlaterLastHittedPoints.Add(_lastPoint, IndexType.Default);
                        if (_secondPlaterNextProbablyPoints.Any())
                        {
                            CheckDictionaries(playerTurn);
                        }
                        break;
                    case PointStatus.Hitted:
                        _secondPlaterLastHittedPoints.Add(_lastPoint, IndexType.Default);
                        CheckDictionaries(playerTurn);
                        break;
                    case PointStatus.Sunk:
                        _secondPlaterLastHittedPoints.Add(_lastPoint, IndexType.Default);
                        _secondPlaterNextProbablyPoints.Clear();
                        break;
                }
            }
        }

        private void CheckDictionaries(bool playerTurn)
        {
            if (playerTurn)
            {
                if (_firstPlaterNextProbablyPoints.Any())
                {
                    UpdateDictionaries(playerTurn);
                }
                else
                {
                    CreateDictionary(playerTurn);
                }
            }
            else
            {
                if (_secondPlaterNextProbablyPoints.Any())
                {
                    UpdateDictionaries(playerTurn);
                }
                else
                {
                    CreateDictionary(playerTurn);
                }
            }
        }

        private void CreateDictionary(bool playerTurn)
        {
            int xIndex = Array.IndexOf(CommonVariables.DefaultXAxis, _lastPoint.X);
            int yIndex = Array.IndexOf(CommonVariables.DefaultYAxis, _lastPoint.Y);
            if (playerTurn)
            {
                if(yIndex != CommonVariables.FirstIndexOfX_Y_Axis)
                {
                    yIndex--;
                    _firstPlaterNextProbablyPoints.Add(Point.CreatePoint(CommonVariables.DefaultXAxis[xIndex], CommonVariables.DefaultYAxis[yIndex]), IndexType.VerticalDown);
                }

                if (yIndex != CommonVariables.LastIndexOfX_Y_Axis)
                {
                    yIndex++;
                    _firstPlaterNextProbablyPoints.Add(Point.CreatePoint(CommonVariables.DefaultXAxis[xIndex], CommonVariables.DefaultYAxis[yIndex]), IndexType.VerticalUp);
                }

                if (xIndex != CommonVariables.FirstIndexOfX_Y_Axis)
                {
                    xIndex--;
                    _firstPlaterNextProbablyPoints.Add(Point.CreatePoint(CommonVariables.DefaultXAxis[xIndex], CommonVariables.DefaultYAxis[yIndex]), IndexType.HorizontalLeft);
                }

                if (xIndex != CommonVariables.LastIndexOfX_Y_Axis)
                {
                    xIndex++;
                    _firstPlaterNextProbablyPoints.Add(Point.CreatePoint(CommonVariables.DefaultXAxis[xIndex], CommonVariables.DefaultYAxis[yIndex]), IndexType.HorizontalRight);
                }
            }
            else
            {
                if (yIndex != CommonVariables.FirstIndexOfX_Y_Axis)
                {
                    yIndex--;
                    _secondPlaterNextProbablyPoints.Add(Point.CreatePoint(CommonVariables.DefaultXAxis[xIndex], CommonVariables.DefaultYAxis[yIndex]), IndexType.VerticalDown);
                }

                if (yIndex != CommonVariables.LastIndexOfX_Y_Axis)
                {
                    yIndex++;
                    _secondPlaterNextProbablyPoints.Add(Point.CreatePoint(CommonVariables.DefaultXAxis[xIndex], CommonVariables.DefaultYAxis[yIndex]), IndexType.VerticalUp);
                }

                if (xIndex != CommonVariables.FirstIndexOfX_Y_Axis)
                {
                    xIndex--;
                    _secondPlaterNextProbablyPoints.Add(Point.CreatePoint(CommonVariables.DefaultXAxis[xIndex], CommonVariables.DefaultYAxis[yIndex]), IndexType.HorizontalLeft);
                }

                if (xIndex != CommonVariables.LastIndexOfX_Y_Axis)
                {
                    xIndex++;
                    _secondPlaterNextProbablyPoints.Add(Point.CreatePoint(CommonVariables.DefaultXAxis[xIndex], CommonVariables.DefaultYAxis[yIndex]), IndexType.HorizontalRight);
                }
            }
        }

        private void UpdateDictionaries(bool playerTurn)
        {
            IndexType indexType;
            int xIndex = Array.IndexOf(CommonVariables.DefaultXAxis, _lastPoint.X);
            int yIndex = Array.IndexOf(CommonVariables.DefaultYAxis, _lastPoint.Y);
            if (playerTurn)
            {
                indexType = _firstPlaterNextProbablyPoints[_lastPoint];
                switch(indexType)
                {
                    case IndexType.VerticalDown:
                        foreach(var item in _firstPlaterNextProbablyPoints.Where(i => i.Value == IndexType.HorizontalLeft || i.Value == IndexType.HorizontalRight))
                        {
                            _firstPlaterNextProbablyPoints.Remove(item.Key);
                        }

                        if(yIndex != CommonVariables.FirstIndexOfX_Y_Axis)
                        {
                            yIndex--;
                            _firstPlaterNextProbablyPoints.Add(Point.CreatePoint(CommonVariables.DefaultXAxis[xIndex], CommonVariables.DefaultYAxis[yIndex]), IndexType.VerticalDown);
                        }
                        break;
                    case IndexType.VerticalUp:
                        foreach (var item in _firstPlaterNextProbablyPoints.Where(i => i.Value == IndexType.HorizontalLeft || i.Value == IndexType.HorizontalRight))
                        {
                            _firstPlaterNextProbablyPoints.Remove(item.Key);
                        }

                        if (yIndex != CommonVariables.LastIndexOfX_Y_Axis)
                        {
                            yIndex++;
                            _firstPlaterNextProbablyPoints.Add(Point.CreatePoint(CommonVariables.DefaultXAxis[xIndex], CommonVariables.DefaultYAxis[yIndex]), IndexType.VerticalUp);
                        }
                        break;
                    case IndexType.HorizontalLeft:
                        foreach (var item in _firstPlaterNextProbablyPoints.Where(i => i.Value == IndexType.VerticalDown || i.Value == IndexType.VerticalUp))
                        {
                            _firstPlaterNextProbablyPoints.Remove(item.Key);
                        }

                        if (xIndex != CommonVariables.FirstIndexOfX_Y_Axis)
                        {
                            xIndex--;
                            _firstPlaterNextProbablyPoints.Add(Point.CreatePoint(CommonVariables.DefaultXAxis[xIndex], CommonVariables.DefaultYAxis[yIndex]), IndexType.HorizontalLeft);
                        }

                        break;
                    case IndexType.HorizontalRight:
                        foreach (var item in _firstPlaterNextProbablyPoints.Where(i => i.Value == IndexType.VerticalDown || i.Value == IndexType.VerticalUp))
                        {
                            _firstPlaterNextProbablyPoints.Remove(item.Key);
                        }

                        if (xIndex != CommonVariables.LastIndexOfX_Y_Axis)
                        {
                            xIndex++;
                            _firstPlaterNextProbablyPoints.Add(Point.CreatePoint(CommonVariables.DefaultXAxis[xIndex], CommonVariables.DefaultYAxis[yIndex]), IndexType.HorizontalRight);
                        }
                        break;
                }
                _firstPlaterNextProbablyPoints.Remove(_lastPoint);
            }
            else
            {
                indexType = _secondPlaterNextProbablyPoints[_lastPoint];
                switch (indexType)
                {
                    case IndexType.VerticalDown:
                        foreach (var item in _secondPlaterNextProbablyPoints.Where(i => i.Value == IndexType.HorizontalLeft || i.Value == IndexType.HorizontalRight))
                        {
                            _secondPlaterNextProbablyPoints.Remove(item.Key);
                        }

                        if (yIndex != CommonVariables.FirstIndexOfX_Y_Axis)
                        {
                            yIndex--;
                            _secondPlaterNextProbablyPoints.Add(Point.CreatePoint(CommonVariables.DefaultXAxis[xIndex], CommonVariables.DefaultYAxis[yIndex]), IndexType.VerticalDown);
                        }
                        break;
                    case IndexType.VerticalUp:
                        foreach (var item in _secondPlaterNextProbablyPoints.Where(i => i.Value == IndexType.HorizontalLeft || i.Value == IndexType.HorizontalRight))
                        {
                            _secondPlaterNextProbablyPoints.Remove(item.Key);
                        }

                        if (yIndex != CommonVariables.LastIndexOfX_Y_Axis)
                        {
                            yIndex++;
                            _secondPlaterNextProbablyPoints.Add(Point.CreatePoint(CommonVariables.DefaultXAxis[xIndex], CommonVariables.DefaultYAxis[yIndex]), IndexType.VerticalUp);
                        }
                        break;
                    case IndexType.HorizontalLeft:
                        foreach (var item in _secondPlaterNextProbablyPoints.Where(i => i.Value == IndexType.VerticalDown || i.Value == IndexType.VerticalUp))
                        {
                            _secondPlaterNextProbablyPoints.Remove(item.Key);
                        }

                        if (xIndex != CommonVariables.FirstIndexOfX_Y_Axis)
                        {
                            xIndex--;
                            _secondPlaterNextProbablyPoints.Add(Point.CreatePoint(CommonVariables.DefaultXAxis[xIndex], CommonVariables.DefaultYAxis[yIndex]), IndexType.HorizontalLeft);
                        }

                        break;
                    case IndexType.HorizontalRight:
                        foreach (var item in _secondPlaterNextProbablyPoints.Where(i => i.Value == IndexType.VerticalDown || i.Value == IndexType.VerticalUp))
                        {
                            _secondPlaterNextProbablyPoints.Remove(item.Key);
                        }

                        if (xIndex != CommonVariables.LastIndexOfX_Y_Axis)
                        {
                            xIndex++;
                            _secondPlaterNextProbablyPoints.Add(Point.CreatePoint(CommonVariables.DefaultXAxis[xIndex], CommonVariables.DefaultYAxis[yIndex]), IndexType.HorizontalRight);
                        }
                        break;
                }
                _secondPlaterNextProbablyPoints.Remove(_lastPoint);
            }
        }
    }
}
