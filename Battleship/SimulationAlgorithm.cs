using CommonObjects;
using MainObjects;

namespace Battleship
{
    internal class SimulationAlgorithm
    {
        private readonly Random _random = new Random();
        private readonly Dictionary<ValueTuple<int, char>, IndexType> _firstPlayerLastHittedPoints;
        private readonly Dictionary<ValueTuple<int, char>, IndexType> _firstPlayerNextProbablyPoints;
        private readonly Dictionary<ValueTuple<int, char>, IndexType> _firstPlayerPointsToShoot;
        private readonly Dictionary<ValueTuple<int, char>, IndexType> _secondPlayerLastHittedPoints;
        private readonly Dictionary<ValueTuple<int, char>, IndexType> _secondPlayerNextProbablyPoints;
        private readonly Dictionary<ValueTuple<int, char>, IndexType> _secondPlayerPointsToShoot;
        private bool _lastTurn;
        private ValueTuple<int, char> _lastPoint = ValueTuple.Create(int.MinValue, char.MinValue);

        public SimulationAlgorithm()
        {
            _firstPlayerLastHittedPoints = new Dictionary<ValueTuple<int, char>, IndexType>();
            _firstPlayerNextProbablyPoints = new Dictionary<ValueTuple<int, char>, IndexType>();
            _secondPlayerLastHittedPoints = new Dictionary<ValueTuple<int, char>, IndexType>();
            _secondPlayerNextProbablyPoints = new Dictionary<ValueTuple<int, char>, IndexType>();
            _firstPlayerPointsToShoot = new Dictionary<(int, char), IndexType>();
            _secondPlayerPointsToShoot = new Dictionary<(int, char), IndexType>();
        }

        internal void Start(Player playerOne, Player playerTwo)
        {
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

            PointStatus pointStatus = PointStatus.Free;

            pointStatus = ShootAndCheckIfHitted(playerOne, playerTwo, playerTurn, pointStatus);

            _lastTurn = playerTurn;

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
                pointStatus = ShootAndCheckIfHitted(playerOne, playerTwo, playerTurn, pointStatus);
                _lastTurn = playerTurn;
            }
        }

        private void FillDictionaries()
        {
            foreach (char y in CommonVariables.DefaultYAxis)
            {
                foreach (int x in CommonVariables.DefaultXAxis)
                {
                    _firstPlayerPointsToShoot.Add((x, y), IndexType.Default);
                    _secondPlayerPointsToShoot.Add((x, y), IndexType.Default);
                }
            }
        }

        private PointStatus ShootAndCheckIfHitted(Player playerOne, Player playerTwo, bool playerTurn, PointStatus pointStatus)
        {
            if (playerTurn)
            {
                pointStatus = FirstPlayerShot(playerOne, playerTwo, playerTurn, pointStatus);
                pointStatus = CheckIfShotHittedForFirstPlayer(playerOne, playerTwo, pointStatus);
                CheckPointStatusForFirstPlayer(pointStatus, playerTurn);
            }
            else
            {
                pointStatus = SecondPlayerShot(playerOne, playerTwo, playerTurn, pointStatus);
                pointStatus = CheckIfShotHittedForSecondPlayer(playerOne, playerTwo, pointStatus);
                CheckPointStatusForSecondPlayer(pointStatus, playerTurn);
            }

            return pointStatus;
        }

        private PointStatus FirstPlayerShot(Player playerOne, Player playerTwo, bool playerTurn, PointStatus pointStatus)
        {
            ValueTuple<int, char> point = CreatePointForFirstPlayer();
            _lastPoint = point;
            return playerOne.Shot(point, playerTwo.Ships);
        }

        private PointStatus SecondPlayerShot(Player playerOne, Player playerTwo, bool playerTurn, PointStatus pointStatus)
        {
            ValueTuple<int, char> point = CreatePointForSecondPlayer();
            _lastPoint = point;
            return playerTwo.Shot(point, playerOne.Ships);
        }
                
        private ValueTuple<int, char> CreatePointForFirstPlayer()
        {
            int index = _random.Next(_firstPlayerNextProbablyPoints.Count);
            if (_firstPlayerNextProbablyPoints.Count == CommonVariables.Zero)
            {
                return CreateNewPointForFirstPlayer();
            }
            while (_firstPlayerLastHittedPoints.ContainsKey(_firstPlayerNextProbablyPoints.ElementAt(index).Key))
            {
                _firstPlayerNextProbablyPoints.Remove(_firstPlayerNextProbablyPoints.ElementAt(index).Key);
                if (_firstPlayerNextProbablyPoints.Count == CommonVariables.Zero)
                {
                    return CreateNewPointForFirstPlayer();
                }
                index = _random.Next(_firstPlayerNextProbablyPoints.Count);
            }
            return _firstPlayerNextProbablyPoints.ElementAt(index).Key;
        }

        private ValueTuple<int, char> CreatePointForSecondPlayer()
        {
            int index = _random.Next(_secondPlayerNextProbablyPoints.Count);
            if (_secondPlayerNextProbablyPoints.Count == CommonVariables.Zero)
            {
                return CreateNewPointForSecondPlayer();
            }
            while (_secondPlayerLastHittedPoints.ContainsKey(_secondPlayerNextProbablyPoints.ElementAt(index).Key))
            {
                _secondPlayerNextProbablyPoints.Remove(_secondPlayerNextProbablyPoints.ElementAt(index).Key);
                if (_secondPlayerNextProbablyPoints.Count == CommonVariables.Zero)
                {
                    return CreateNewPointForSecondPlayer();
                }
                index = _random.Next(_secondPlayerNextProbablyPoints.Count);
            }
            return _secondPlayerNextProbablyPoints.ElementAt(index).Key;
        }

        private ValueTuple<int, char> CreateNewPointForFirstPlayer()
        {
            ValueTuple<int, char> result = _firstPlayerPointsToShoot.ElementAt(_random.Next(_firstPlayerPointsToShoot.Count)).Key;
            while (_firstPlayerLastHittedPoints.ContainsKey(result))
            {
                result = _firstPlayerPointsToShoot.ElementAt(_random.Next(_firstPlayerPointsToShoot.Count)).Key;
            }
            _firstPlayerPointsToShoot.Remove(result);
            return result;
        }

        private ValueTuple<int, char> CreateNewPointForSecondPlayer()
        {
            ValueTuple<int, char> result = _secondPlayerPointsToShoot.ElementAt(_random.Next(_secondPlayerPointsToShoot.Count)).Key;
            while (_secondPlayerLastHittedPoints.ContainsKey(result))
            {
                result = _secondPlayerPointsToShoot.ElementAt(_random.Next(_secondPlayerPointsToShoot.Count)).Key;
            }
            _secondPlayerPointsToShoot.Remove(result);
            return result;
        }
        
        private PointStatus CheckIfShotHittedForFirstPlayer(Player playerOne, Player playerTwo, PointStatus pointStatus)
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

        private PointStatus CheckIfShotHittedForSecondPlayer(Player playerOne, Player playerTwo, PointStatus pointStatus)
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

        private void CheckPointStatusForFirstPlayer(PointStatus pointStatus, bool playerTurn)
        {
            switch (pointStatus)
            {
                case PointStatus.Missed:
                    _firstPlayerLastHittedPoints.Add(_lastPoint, IndexType.Default);
                    if (_firstPlayerNextProbablyPoints.Any())
                    {
                        CheckDictionariesForFirstPlayer();
                    }
                    break;
                case PointStatus.Hitted:
                    _firstPlayerLastHittedPoints.Add(_lastPoint, IndexType.Default);
                    CheckDictionariesForFirstPlayer();
                    break;
                case PointStatus.Sunk:
                    _firstPlayerLastHittedPoints.Add(_lastPoint, IndexType.Default);
                    _firstPlayerNextProbablyPoints.Clear();
                    break;
            }
        }

        private void CheckPointStatusForSecondPlayer(PointStatus pointStatus, bool playerTurn)
        {
            switch (pointStatus)
            {
                case PointStatus.Missed:
                    _secondPlayerLastHittedPoints.Add(_lastPoint, IndexType.Default);
                    if (_secondPlayerNextProbablyPoints.Any())
                    {
                        CheckDictionariesForSecondPlayer();
                    }
                    break;
                case PointStatus.Hitted:
                    _secondPlayerLastHittedPoints.Add(_lastPoint, IndexType.Default);
                    CheckDictionariesForSecondPlayer();
                    break;
                case PointStatus.Sunk:
                    _secondPlayerLastHittedPoints.Add(_lastPoint, IndexType.Default);
                    _secondPlayerNextProbablyPoints.Clear();
                    break;
            }
        }

        private void CheckDictionariesForFirstPlayer()
        {
            if (_firstPlayerNextProbablyPoints.Any())
            {
                UpdateDictionariesForFirstPlayer();
            }
            else
            {
                CreateDictionaryForFirstPlayer();
            }
        }

        private void CheckDictionariesForSecondPlayer()
        {
            if (_secondPlayerNextProbablyPoints.Any())
            {
                UpdateDictionariesForSecondPlayer();
            }
            else
            {
                CreateDictionaryForSecondPlayer();
            }
        }

        private void CreateDictionaryForFirstPlayer()
        {
            int xIndex = Array.IndexOf(CommonVariables.DefaultXAxis, _lastPoint.Item1);
            int yIndex = Array.IndexOf(CommonVariables.DefaultYAxis, _lastPoint.Item2);
            int xIndexCopy = xIndex;
            int yIndexCopy = yIndex;

            if (yIndex != CommonVariables.FirstIndexOfX_Y_Axis)
            {
                yIndexCopy--;
                _firstPlayerNextProbablyPoints.Add(ValueTuple.Create(CommonVariables.DefaultXAxis[xIndex], CommonVariables.DefaultYAxis[yIndexCopy]), IndexType.VerticalDown);
            }

            yIndexCopy = yIndex;

            if (yIndex != CommonVariables.LastIndexOfX_Y_Axis)
            {
                yIndexCopy++;
                _firstPlayerNextProbablyPoints.Add(ValueTuple.Create(CommonVariables.DefaultXAxis[xIndex], CommonVariables.DefaultYAxis[yIndexCopy]), IndexType.VerticalUp);
            }

            if (xIndex != CommonVariables.FirstIndexOfX_Y_Axis)
            {
                xIndexCopy--;
                _firstPlayerNextProbablyPoints.Add(ValueTuple.Create(CommonVariables.DefaultXAxis[xIndexCopy], CommonVariables.DefaultYAxis[yIndex]), IndexType.HorizontalLeft);
            }

            xIndexCopy = xIndex;

            if (xIndex != CommonVariables.LastIndexOfX_Y_Axis)
            {
                xIndexCopy++;
                _firstPlayerNextProbablyPoints.Add(ValueTuple.Create(CommonVariables.DefaultXAxis[xIndexCopy], CommonVariables.DefaultYAxis[yIndex]), IndexType.HorizontalRight);
            }
        }

        private void CreateDictionaryForSecondPlayer()
        {
            int xIndex = Array.IndexOf(CommonVariables.DefaultXAxis, _lastPoint.Item1);
            int yIndex = Array.IndexOf(CommonVariables.DefaultYAxis, _lastPoint.Item2);
            int xIndexCopy = xIndex;
            int yIndexCopy = yIndex;

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

        private void UpdateDictionariesForFirstPlayer()
        {
            IndexType indexType = _firstPlayerNextProbablyPoints[_lastPoint];
            int xIndex = Array.IndexOf(CommonVariables.DefaultXAxis, _lastPoint.Item1);
            int yIndex = Array.IndexOf(CommonVariables.DefaultYAxis, _lastPoint.Item2);

            switch (indexType)
            {
                case IndexType.VerticalDown:
                    foreach (var item in _firstPlayerNextProbablyPoints.Where(i => i.Value == IndexType.HorizontalLeft || i.Value == IndexType.HorizontalRight))
                    {
                        _firstPlayerNextProbablyPoints.Remove(item.Key);
                    }

                    if (yIndex != CommonVariables.FirstIndexOfX_Y_Axis)
                    {
                        yIndex--;
                        _firstPlayerNextProbablyPoints.Add(ValueTuple.Create(CommonVariables.DefaultXAxis[xIndex], CommonVariables.DefaultYAxis[yIndex]), IndexType.VerticalDown);
                    }
                    break;
                case IndexType.VerticalUp:
                    foreach (var item in _firstPlayerNextProbablyPoints.Where(i => i.Value == IndexType.HorizontalLeft || i.Value == IndexType.HorizontalRight))
                    {
                        _firstPlayerNextProbablyPoints.Remove(item.Key);
                    }

                    if (yIndex != CommonVariables.LastIndexOfX_Y_Axis)
                    {
                        yIndex++;
                        _firstPlayerNextProbablyPoints.Add(ValueTuple.Create(CommonVariables.DefaultXAxis[xIndex], CommonVariables.DefaultYAxis[yIndex]), IndexType.VerticalUp);
                    }
                    break;
                case IndexType.HorizontalLeft:
                    foreach (var item in _firstPlayerNextProbablyPoints.Where(i => i.Value == IndexType.VerticalDown || i.Value == IndexType.VerticalUp))
                    {
                        _firstPlayerNextProbablyPoints.Remove(item.Key);
                    }

                    if (xIndex != CommonVariables.FirstIndexOfX_Y_Axis)
                    {
                        xIndex--;
                        _firstPlayerNextProbablyPoints.Add(ValueTuple.Create(CommonVariables.DefaultXAxis[xIndex], CommonVariables.DefaultYAxis[yIndex]), IndexType.HorizontalLeft);
                    }

                    break;
                case IndexType.HorizontalRight:
                    foreach (var item in _firstPlayerNextProbablyPoints.Where(i => i.Value == IndexType.VerticalDown || i.Value == IndexType.VerticalUp))
                    {
                        _firstPlayerNextProbablyPoints.Remove(item.Key);
                    }

                    if (xIndex != CommonVariables.LastIndexOfX_Y_Axis)
                    {
                        xIndex++;
                        _firstPlayerNextProbablyPoints.Add(ValueTuple.Create(CommonVariables.DefaultXAxis[xIndex], CommonVariables.DefaultYAxis[yIndex]), IndexType.HorizontalRight);
                    }
                    break;
            }
            _firstPlayerNextProbablyPoints.Remove(_lastPoint);
        }

        private void UpdateDictionariesForSecondPlayer()
        {
            IndexType indexType = _secondPlayerNextProbablyPoints[_lastPoint];
            int xIndex = Array.IndexOf(CommonVariables.DefaultXAxis, _lastPoint.Item1);
            int yIndex = Array.IndexOf(CommonVariables.DefaultYAxis, _lastPoint.Item2);

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
