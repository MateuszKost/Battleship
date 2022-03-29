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
        private readonly Dictionary<ValueTuple<int, char>, IndexType> _secondPlaterLastHittedPoints;
        private readonly Dictionary<ValueTuple<int, char>, IndexType> _secondPlaterNextProbablyPoints;

        public SimulationAlgorithm()
        {
            _firstPlaterLastHittedPoints = new Dictionary<ValueTuple<int, char>, IndexType>();
            _firstPlaterNextProbablyPoints = new Dictionary<ValueTuple<int, char>, IndexType>();
            _secondPlaterLastHittedPoints = new Dictionary<ValueTuple<int, char>, IndexType>();
            _secondPlaterNextProbablyPoints = new Dictionary<ValueTuple<int, char>, IndexType>();
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
            ValueTuple<int, char> point;
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

        private ValueTuple<int, char> WhereToShot(Player player, bool playerTurn, PointStatus pointStatus)
        {
            if ((_lastPoint != (-1, '0') && _lastTurn == playerTurn && pointStatus != PointStatus.Sunk) || player.HittedButNotSunk)
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

        private ValueTuple<int, char> CreatePoint()
        {
            int xIndex = _random.Next(CommonVariables.LastIndexOfX_Y_Axis);
            int yIndex = _random.Next(CommonVariables.LastIndexOfX_Y_Axis);

            return ValueTuple.Create(CommonVariables.DefaultXAxis[xIndex], CommonVariables.DefaultYAxis[yIndex]);
        }

        private ValueTuple<int, char> CreateNearPoint(bool playerTurn)
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
            int xIndex = Array.IndexOf(CommonVariables.DefaultXAxis, _lastPoint.Item1);
            int yIndex = Array.IndexOf(CommonVariables.DefaultYAxis, _lastPoint.Item2);
            int xIndexCopy = xIndex;
            int yIndexCopy = yIndex;


            if (playerTurn)
            {
                if(yIndex != CommonVariables.FirstIndexOfX_Y_Axis)
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
                    _secondPlaterNextProbablyPoints.Add(ValueTuple.Create(CommonVariables.DefaultXAxis[xIndex], CommonVariables.DefaultYAxis[yIndexCopy]), IndexType.VerticalDown);
                }

                yIndexCopy = yIndex;

                if (yIndex != CommonVariables.LastIndexOfX_Y_Axis)
                {
                    yIndex++;
                    _secondPlaterNextProbablyPoints.Add(ValueTuple.Create(CommonVariables.DefaultXAxis[xIndex], CommonVariables.DefaultYAxis[yIndexCopy]), IndexType.VerticalUp);
                }

                if (xIndex != CommonVariables.FirstIndexOfX_Y_Axis)
                {
                    xIndexCopy--;
                    _secondPlaterNextProbablyPoints.Add(ValueTuple.Create(CommonVariables.DefaultXAxis[xIndexCopy], CommonVariables.DefaultYAxis[yIndex]), IndexType.HorizontalLeft);
                }

                xIndexCopy = xIndex;

                if (xIndex != CommonVariables.LastIndexOfX_Y_Axis)
                {
                    xIndexCopy++;
                    _secondPlaterNextProbablyPoints.Add(ValueTuple.Create(CommonVariables.DefaultXAxis[xIndexCopy], CommonVariables.DefaultYAxis[yIndex]), IndexType.HorizontalRight);
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
                            _secondPlaterNextProbablyPoints.Add(ValueTuple.Create(CommonVariables.DefaultXAxis[xIndex], CommonVariables.DefaultYAxis[yIndex]), IndexType.VerticalDown);
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
                            _secondPlaterNextProbablyPoints.Add(ValueTuple.Create(CommonVariables.DefaultXAxis[xIndex], CommonVariables.DefaultYAxis[yIndex]), IndexType.VerticalUp);
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
                            _secondPlaterNextProbablyPoints.Add(ValueTuple.Create(CommonVariables.DefaultXAxis[xIndex], CommonVariables.DefaultYAxis[yIndex]), IndexType.HorizontalLeft);
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
                            _secondPlaterNextProbablyPoints.Add(ValueTuple.Create(CommonVariables.DefaultXAxis[xIndex], CommonVariables.DefaultYAxis[yIndex]), IndexType.HorizontalRight);
                        }
                        break;
                }
                _secondPlaterNextProbablyPoints.Remove(_lastPoint);
            }
        }
    }
}
