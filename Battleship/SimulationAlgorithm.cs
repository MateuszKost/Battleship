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
                CheckPointStatusForFirstPlayer(pointStatus);
            }
            else
            {
                pointStatus = SecondPlayerShot(playerOne, playerTwo, playerTurn, pointStatus);
                pointStatus = CheckIfShotHittedForSecondPlayer(playerOne, playerTwo, pointStatus);
                CheckPointStatusForSecondPlayer(pointStatus);
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

        private void CheckPointStatusForFirstPlayer(PointStatus pointStatus)
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

        private void CheckPointStatusForSecondPlayer(PointStatus pointStatus)
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
            FindIndexes(out int xIndex, out int yIndex);
            int xIndexCopy = xIndex;
            int yIndexCopy = yIndex;

            TryToAddPointToNextProbablyPointsForFirstPlayer(IndexType.VerticalDown, xIndex, yIndexCopy);
            yIndexCopy = yIndex;
            TryToAddPointToNextProbablyPointsForFirstPlayer(IndexType.VerticalUp, xIndex, yIndexCopy);
            TryToAddPointToNextProbablyPointsForFirstPlayer(IndexType.HorizontalLeft, xIndexCopy, yIndex);
            xIndexCopy = xIndex;
            TryToAddPointToNextProbablyPointsForFirstPlayer(IndexType.HorizontalRight, xIndexCopy, yIndex);
        }

        private void CreateDictionaryForSecondPlayer()
        {
            FindIndexes(out int xIndex, out int yIndex);
            int xIndexCopy = xIndex;
            int yIndexCopy = yIndex;

            TryToAddPointToNextProbablyPointsForSecondPlayer(IndexType.VerticalDown, xIndex, yIndexCopy);
            yIndexCopy = yIndex;
            TryToAddPointToNextProbablyPointsForSecondPlayer(IndexType.VerticalUp, xIndex, yIndexCopy);
            TryToAddPointToNextProbablyPointsForSecondPlayer(IndexType.HorizontalLeft, xIndexCopy, yIndex);
            xIndexCopy = xIndex;
            TryToAddPointToNextProbablyPointsForSecondPlayer(IndexType.HorizontalRight, xIndexCopy, yIndex);
        }

        private void UpdateDictionariesForFirstPlayer()
        {
            IndexType indexType = _firstPlayerNextProbablyPoints[_lastPoint];
            FindIndexes(out int xIndex, out int yIndex);

            switch (indexType)
            {
                case IndexType.VerticalDown:
                    foreach (var item in _firstPlayerNextProbablyPoints.Where(i => i.Value == IndexType.HorizontalLeft || i.Value == IndexType.HorizontalRight))
                    {
                        _firstPlayerNextProbablyPoints.Remove(item.Key);
                    }
                    break;
                case IndexType.VerticalUp:
                    foreach (var item in _firstPlayerNextProbablyPoints.Where(i => i.Value == IndexType.HorizontalLeft || i.Value == IndexType.HorizontalRight))
                    {
                        _firstPlayerNextProbablyPoints.Remove(item.Key);
                    }
                    break;
                case IndexType.HorizontalLeft:
                    foreach (var item in _firstPlayerNextProbablyPoints.Where(i => i.Value == IndexType.VerticalDown || i.Value == IndexType.VerticalUp))
                    {
                        _firstPlayerNextProbablyPoints.Remove(item.Key);
                    }
                    break;
                case IndexType.HorizontalRight:
                    foreach (var item in _firstPlayerNextProbablyPoints.Where(i => i.Value == IndexType.VerticalDown || i.Value == IndexType.VerticalUp))
                    {
                        _firstPlayerNextProbablyPoints.Remove(item.Key);
                    }
                    break;
            }

            TryToAddPointToNextProbablyPointsForFirstPlayer(indexType, xIndex, yIndex);
            _firstPlayerNextProbablyPoints.Remove(_lastPoint);
        }

        private void UpdateDictionariesForSecondPlayer()
        {
            IndexType indexType = _secondPlayerNextProbablyPoints[_lastPoint];
            FindIndexes(out int xIndex, out int yIndex);

            switch (indexType)
            {
                case IndexType.VerticalDown:
                    foreach (var item in _secondPlayerNextProbablyPoints.Where(i => i.Value == IndexType.HorizontalLeft || i.Value == IndexType.HorizontalRight))
                    {
                        _secondPlayerNextProbablyPoints.Remove(item.Key);
                    }
                    break;
                case IndexType.VerticalUp:
                    foreach (var item in _secondPlayerNextProbablyPoints.Where(i => i.Value == IndexType.HorizontalLeft || i.Value == IndexType.HorizontalRight))
                    {
                        _secondPlayerNextProbablyPoints.Remove(item.Key);
                    }
                    break;
                case IndexType.HorizontalLeft:
                    foreach (var item in _secondPlayerNextProbablyPoints.Where(i => i.Value == IndexType.VerticalDown || i.Value == IndexType.VerticalUp))
                    {
                        _secondPlayerNextProbablyPoints.Remove(item.Key);
                    }
                    break;
                case IndexType.HorizontalRight:
                    foreach (var item in _secondPlayerNextProbablyPoints.Where(i => i.Value == IndexType.VerticalDown || i.Value == IndexType.VerticalUp))
                    {
                        _secondPlayerNextProbablyPoints.Remove(item.Key);
                    }
                    break;
            }

            TryToAddPointToNextProbablyPointsForSecondPlayer(indexType, xIndex, yIndex);
            _secondPlayerNextProbablyPoints.Remove(_lastPoint);
        }

        private void TryToAddPointToNextProbablyPointsForFirstPlayer(IndexType indexType, int xIndex, int yIndex)
        {
            if (yIndex != CommonVariables.FirstIndexOfX_Y_Axis && yIndex != CommonVariables.LastIndexOfX_Y_Axis && xIndex != CommonVariables.FirstIndexOfX_Y_Axis && xIndex != CommonVariables.LastIndexOfX_Y_Axis)
            {
                DecrementOrIncrementIndex(indexType, xIndex, yIndex, out xIndex, out yIndex);
                _firstPlayerNextProbablyPoints.Add(ValueTuple.Create(CommonVariables.DefaultXAxis[xIndex], CommonVariables.DefaultYAxis[yIndex]), indexType);
            }
        }

        private void TryToAddPointToNextProbablyPointsForSecondPlayer(IndexType indexType, int xIndex, int yIndex)
        {
            if (yIndex != CommonVariables.FirstIndexOfX_Y_Axis && yIndex != CommonVariables.LastIndexOfX_Y_Axis && xIndex != CommonVariables.FirstIndexOfX_Y_Axis && xIndex != CommonVariables.LastIndexOfX_Y_Axis)
            {
                DecrementOrIncrementIndex(indexType, xIndex, yIndex, out xIndex, out yIndex);
                _secondPlayerNextProbablyPoints.Add(ValueTuple.Create(CommonVariables.DefaultXAxis[xIndex], CommonVariables.DefaultYAxis[yIndex]), indexType);
            }
        }

        private void FindIndexes(out int xIndex, out int yIndex)
        {
            xIndex = Array.IndexOf(CommonVariables.DefaultXAxis, _lastPoint.Item1);
            yIndex = Array.IndexOf(CommonVariables.DefaultYAxis, _lastPoint.Item2);
        }

        private void DecrementOrIncrementIndex(IndexType indexType, int xIndex, int yIndex, out int newXIndex, out int newYIndex)
        {
            switch (indexType)
            {
                case IndexType.VerticalDown:
                    yIndex--;
                    break;
                case IndexType.VerticalUp:
                    yIndex++;
                    break;
                case IndexType.HorizontalLeft:
                    xIndex--;
                    break;
                case IndexType.HorizontalRight:
                    xIndex++;
                    break;
            }
            newXIndex = xIndex;
            newYIndex = yIndex;
        }
    }
}
