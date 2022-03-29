using CommonObjects;
using MainObjects;

namespace Battleship
{
    internal class SimulationAlgorithm
    {
        private readonly Random _random = new Random();
        private bool _lastTurn;
        private ValueTuple<int, char> _lastPoint = ValueTuple.Create(-1, ' '); // can i create valuetuple empty
        private readonly Dictionary<ValueTuple<int, char>, IndexType> _firstPlaterLastHittedPoints;
        private readonly Dictionary<ValueTuple<int, char>, IndexType> _firstPlaterNextProbablyPoints;
        private readonly Dictionary<ValueTuple<int, char>, IndexType> _firstPlaterPointsToShoot;
        private readonly Dictionary<ValueTuple<int, char>, IndexType> _secondPlayerLastHittedPoints;
        private readonly Dictionary<ValueTuple<int, char>, IndexType> _secondPlayerNextProbablyPoints;
        private readonly Dictionary<ValueTuple<int, char>, IndexType> _secondPlayerPointsToShoot;

        public SimulationAlgorithm()
        {
            _firstPlaterLastHittedPoints = new Dictionary<ValueTuple<int, char>, IndexType>();
            _firstPlaterNextProbablyPoints = new Dictionary<ValueTuple<int, char>, IndexType>();
            _secondPlayerLastHittedPoints = new Dictionary<ValueTuple<int, char>, IndexType>();
            _secondPlayerNextProbablyPoints = new Dictionary<ValueTuple<int, char>, IndexType>();
            _firstPlaterPointsToShoot = new Dictionary<(int, char), IndexType>();
            _secondPlayerPointsToShoot = new Dictionary<(int, char), IndexType>();
        }

        private void FillDictionaries()
        {
            foreach (char y in CommonVariables.DefaultYAxis)
            {
                foreach (int x in CommonVariables.DefaultXAxis)
                {
                    _firstPlaterPointsToShoot.Add((x, y), IndexType.Default);
                    _secondPlayerPointsToShoot.Add((x, y), IndexType.Default);
                }
            }
        }

        internal void Start(Player playerOne, Player playerTwo)
        {
            bool playerTurn;
            FillDictionaries();

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
            pointStatus = CheckIfShotHitted(playerOne, playerTwo, pointStatus, playerTurn);
            CheckPointStatus(pointStatus, playerTurn);

            _lastTurn = playerTurn;

            while (playerOne.Ships.Count != CommonVariables.Zero && playerTwo.Ships.Count != CommonVariables.Zero)
            {
                if (pointStatus == PointStatus.Missed)
                {
                    playerTurn = !playerTurn;
                    if (playerTurn)
                        Console.WriteLine("Ruch gracza 1!");
                    else
                        Console.WriteLine("Ruch gracza 2!");
                }
                pointStatus = Shot(playerOne, playerTwo, playerTurn, pointStatus);
                pointStatus = CheckIfShotHitted(playerOne, playerTwo, pointStatus, playerTurn);
                CheckPointStatus(pointStatus, playerTurn);
                _lastTurn = playerTurn;
            }
        }

        private PointStatus Shot(Player playerOne, Player playerTwo, bool playerTurn, PointStatus pointStatus)
        {
            ValueTuple<int, char> point;
            if (playerTurn)
            {
                point = WhereToShot(playerTwo, playerTurn, pointStatus);
                return playerOne.Shot(point, playerTwo.Ships);
            }
            else
            {
                point = WhereToShot(playerOne, playerTurn, pointStatus);
                return playerTwo.Shot(point, playerOne.Ships);
            }
        }

        private ValueTuple<int, char> WhereToShot(Player player, bool playerTurn, PointStatus pointStatus)
        {
            if ((_lastPoint != (-1, ' ') && _lastTurn == playerTurn && pointStatus != PointStatus.Sunk) || player.HittedButNotSunk)
            {
                _lastPoint = CreateNearPoint(playerTurn, player);
                return _lastPoint;
            }
            else
            {
                _lastPoint = CreatePoint(playerTurn);
                return _lastPoint;
            }
        }

        private ValueTuple<int, char> CreatePoint(bool playerTurn)
        {
            ValueTuple<int, char> result;
            if (playerTurn)
            {
                result = _firstPlaterPointsToShoot.ElementAt(_random.Next(_firstPlaterPointsToShoot.Count())).Key;
                while (_firstPlaterLastHittedPoints.ContainsKey(result))
                {
                    result = _firstPlaterPointsToShoot.ElementAt(_random.Next(_firstPlaterPointsToShoot.Count())).Key;
                }
                _firstPlaterPointsToShoot.Remove(result);
            }
            else
            {
                result = _secondPlayerPointsToShoot.ElementAt(_random.Next(_secondPlayerPointsToShoot.Count())).Key;
                while (_secondPlayerLastHittedPoints.ContainsKey(result))
                {
                    result = _secondPlayerPointsToShoot.ElementAt(_random.Next(_secondPlayerPointsToShoot.Count())).Key;
                }
                _secondPlayerPointsToShoot.Remove(result);
            }

            return result;
        }

        private ValueTuple<int, char> CreateNearPoint(bool playerTurn, Player player)
        {
            int index;
            if (playerTurn)
            {
                index = _random.Next(_firstPlaterNextProbablyPoints.Count);
                if (_firstPlaterNextProbablyPoints.Count == 0)
                {
                    return CreatePoint(playerTurn);
                }
                while (_firstPlaterLastHittedPoints.ContainsKey(_firstPlaterNextProbablyPoints.ElementAt(index).Key))
                {
                    _firstPlaterNextProbablyPoints.Remove(_firstPlaterNextProbablyPoints.ElementAt(index).Key);
                    if(_firstPlaterNextProbablyPoints.Count == 0)
                    {
                        return CreatePoint(playerTurn);
                    }
                    index = _random.Next(_firstPlaterNextProbablyPoints.Count);
                }
                return _firstPlaterNextProbablyPoints.ElementAt(index).Key;
            }
            else
            {
                index = _random.Next(_secondPlayerNextProbablyPoints.Count);
                if (_secondPlayerNextProbablyPoints.Count == 0)
                {
                    return CreatePoint(playerTurn);
                }
                while (_secondPlayerLastHittedPoints.ContainsKey(_secondPlayerNextProbablyPoints.ElementAt(index).Key))
                {
                    _secondPlayerNextProbablyPoints.Remove(_secondPlayerNextProbablyPoints.ElementAt(index).Key);
                    if (_secondPlayerNextProbablyPoints.Count == 0)
                    {
                        return CreatePoint(playerTurn);
                    }
                    index = _random.Next(_secondPlayerNextProbablyPoints.Count);
                }
                return _secondPlayerNextProbablyPoints.ElementAt(index).Key;
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
                        playerTwo.UpdateEnemyMap(_lastPoint, pointStatus);
                        return pointStatus;
                    case PointStatus.Hitted:
                        playerTwo.UpdateEnemyMap(_lastPoint, pointStatus);
                        return playerOne.UpdateOwnMap(_lastPoint);
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
                        _secondPlayerLastHittedPoints.Add(_lastPoint, IndexType.Default);
                        if (_secondPlayerNextProbablyPoints.Any())
                        {
                            CheckDictionaries(playerTurn);
                        }
                        break;
                    case PointStatus.Hitted:
                        _secondPlayerLastHittedPoints.Add(_lastPoint, IndexType.Default);
                        CheckDictionaries(playerTurn);
                        break;
                    case PointStatus.Sunk:
                        _secondPlayerLastHittedPoints.Add(_lastPoint, IndexType.Default);
                        _secondPlayerNextProbablyPoints.Clear();
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
                if (_secondPlayerNextProbablyPoints.Any())
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
            int xIndex = Array.IndexOf(CommonVariables.DefaultXAxis, _lastPoint.Item1);
            int yIndex = Array.IndexOf(CommonVariables.DefaultYAxis, _lastPoint.Item2);
            int xIndexCopy = xIndex;
            int yIndexCopy = yIndex;


            if (playerTurn)
            {
                if (yIndex != CommonVariables.FirstIndexOfX_Y_Axis)
                {
                    yIndexCopy--; //  to rebuild, there can't be such thing as it, use instead of it copy of index and find points near it
                    _firstPlaterNextProbablyPoints.Add(ValueTuple.Create(CommonVariables.DefaultXAxis[xIndex], CommonVariables.DefaultYAxis[yIndexCopy]), IndexType.VerticalDown);
                }

                yIndexCopy = yIndex;

                if (yIndex != CommonVariables.LastIndexOfX_Y_Axis)
                {
                    yIndexCopy++;
                    _firstPlaterNextProbablyPoints.Add(ValueTuple.Create(CommonVariables.DefaultXAxis[xIndex], CommonVariables.DefaultYAxis[yIndexCopy]), IndexType.VerticalUp);
                }

                if (xIndex != CommonVariables.FirstIndexOfX_Y_Axis)
                {
                    xIndexCopy--;
                    _firstPlaterNextProbablyPoints.Add(ValueTuple.Create(CommonVariables.DefaultXAxis[xIndexCopy], CommonVariables.DefaultYAxis[yIndex]), IndexType.HorizontalLeft);
                }

                xIndexCopy = xIndex;

                if (xIndex != CommonVariables.LastIndexOfX_Y_Axis)
                {
                    xIndexCopy++;
                    _firstPlaterNextProbablyPoints.Add(ValueTuple.Create(CommonVariables.DefaultXAxis[xIndexCopy], CommonVariables.DefaultYAxis[yIndex]), IndexType.HorizontalRight);
                }
            }
            else
            {
                if (yIndex != CommonVariables.FirstIndexOfX_Y_Axis)
                {
                    yIndexCopy--;
                    _secondPlayerNextProbablyPoints.Add(ValueTuple.Create(CommonVariables.DefaultXAxis[xIndex], CommonVariables.DefaultYAxis[yIndexCopy]), IndexType.VerticalDown);
                }

                yIndexCopy = yIndex;

                if (yIndex != CommonVariables.LastIndexOfX_Y_Axis)
                {
                    yIndexCopy++;
                    _secondPlayerNextProbablyPoints.Add(ValueTuple.Create(CommonVariables.DefaultXAxis[xIndex], CommonVariables.DefaultYAxis[yIndexCopy]), IndexType.VerticalUp);
                }

                if (xIndex != CommonVariables.FirstIndexOfX_Y_Axis)
                {
                    xIndexCopy--;
                    _secondPlayerNextProbablyPoints.Add(ValueTuple.Create(CommonVariables.DefaultXAxis[xIndexCopy], CommonVariables.DefaultYAxis[yIndex]), IndexType.HorizontalLeft);
                }

                xIndexCopy = xIndex;

                if (xIndex != CommonVariables.LastIndexOfX_Y_Axis)
                {
                    xIndexCopy++;
                    _secondPlayerNextProbablyPoints.Add(ValueTuple.Create(CommonVariables.DefaultXAxis[xIndexCopy], CommonVariables.DefaultYAxis[yIndex]), IndexType.HorizontalRight);
                }
            }
        }

        private void UpdateDictionaries(bool playerTurn)
        {
            IndexType indexType;
            int xIndex = Array.IndexOf(CommonVariables.DefaultXAxis, _lastPoint.Item1);
            int yIndex = Array.IndexOf(CommonVariables.DefaultYAxis, _lastPoint.Item2);
            if (playerTurn)
            {
                indexType = _firstPlaterNextProbablyPoints[_lastPoint];
                switch (indexType)
                {
                    case IndexType.VerticalDown:
                        foreach (var item in _firstPlaterNextProbablyPoints.Where(i => i.Value == IndexType.HorizontalLeft || i.Value == IndexType.HorizontalRight))
                        {
                            _firstPlaterNextProbablyPoints.Remove(item.Key);
                        }

                        if (yIndex != CommonVariables.FirstIndexOfX_Y_Axis)
                        {
                            yIndex--;
                            _firstPlaterNextProbablyPoints.Add(ValueTuple.Create(CommonVariables.DefaultXAxis[xIndex], CommonVariables.DefaultYAxis[yIndex]), IndexType.VerticalDown);
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
                            _firstPlaterNextProbablyPoints.Add(ValueTuple.Create(CommonVariables.DefaultXAxis[xIndex], CommonVariables.DefaultYAxis[yIndex]), IndexType.VerticalUp);
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
                            _firstPlaterNextProbablyPoints.Add(ValueTuple.Create(CommonVariables.DefaultXAxis[xIndex], CommonVariables.DefaultYAxis[yIndex]), IndexType.HorizontalLeft);
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
                            _firstPlaterNextProbablyPoints.Add(ValueTuple.Create(CommonVariables.DefaultXAxis[xIndex], CommonVariables.DefaultYAxis[yIndex]), IndexType.HorizontalRight);
                        }
                        break;
                }
                _firstPlaterNextProbablyPoints.Remove(_lastPoint);
            }
            else
            {
                indexType = _secondPlayerNextProbablyPoints[_lastPoint];
                switch (indexType)
                {
                    case IndexType.VerticalDown:
                        foreach (var item in _secondPlayerNextProbablyPoints.Where(i => i.Value == IndexType.HorizontalLeft || i.Value == IndexType.HorizontalRight))
                        {
                            _secondPlayerNextProbablyPoints.Remove(item.Key);
                        }

                        if (yIndex != CommonVariables.FirstIndexOfX_Y_Axis)
                        {
                            yIndex--;
                            _secondPlayerNextProbablyPoints.Add(ValueTuple.Create(CommonVariables.DefaultXAxis[xIndex], CommonVariables.DefaultYAxis[yIndex]), IndexType.VerticalDown);
                        }
                        break;
                    case IndexType.VerticalUp:
                        foreach (var item in _secondPlayerNextProbablyPoints.Where(i => i.Value == IndexType.HorizontalLeft || i.Value == IndexType.HorizontalRight))
                        {
                            _secondPlayerNextProbablyPoints.Remove(item.Key);
                        }

                        if (yIndex != CommonVariables.LastIndexOfX_Y_Axis)
                        {
                            yIndex++;
                            _secondPlayerNextProbablyPoints.Add(ValueTuple.Create(CommonVariables.DefaultXAxis[xIndex], CommonVariables.DefaultYAxis[yIndex]), IndexType.VerticalUp);
                        }
                        break;
                    case IndexType.HorizontalLeft:
                        foreach (var item in _secondPlayerNextProbablyPoints.Where(i => i.Value == IndexType.VerticalDown || i.Value == IndexType.VerticalUp))
                        {
                            _secondPlayerNextProbablyPoints.Remove(item.Key);
                        }

                        if (xIndex != CommonVariables.FirstIndexOfX_Y_Axis)
                        {
                            xIndex--;
                            _secondPlayerNextProbablyPoints.Add(ValueTuple.Create(CommonVariables.DefaultXAxis[xIndex], CommonVariables.DefaultYAxis[yIndex]), IndexType.HorizontalLeft);
                        }

                        break;
                    case IndexType.HorizontalRight:
                        foreach (var item in _secondPlayerNextProbablyPoints.Where(i => i.Value == IndexType.VerticalDown || i.Value == IndexType.VerticalUp))
                        {
                            _secondPlayerNextProbablyPoints.Remove(item.Key);
                        }

                        if (xIndex != CommonVariables.LastIndexOfX_Y_Axis)
                        {
                            xIndex++;
                            _secondPlayerNextProbablyPoints.Add(ValueTuple.Create(CommonVariables.DefaultXAxis[xIndex], CommonVariables.DefaultYAxis[yIndex]), IndexType.HorizontalRight);
                        }
                        break;
                }
                _secondPlayerNextProbablyPoints.Remove(_lastPoint);
            }
        }
    }
}
