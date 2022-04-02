using CommonObjects;
using MainObjects;

namespace Battleship
{
    public class CreationAlgorithm
    {
        private readonly Random _random = new Random();
        private bool _guard = false;
        private ExtraPoint[] _ownMap;
        private ExtraPoint[] _enemyMap;

        public Player CreatePlayer(string firstPlayerName)
        {
            _ownMap = new ExtraPoint[CommonVariables.DefaultMapSize];
            _enemyMap = new ExtraPoint[CommonVariables.DefaultMapSize];

            FillMaps();

            return Player.CreatePlayer(firstPlayerName, CreateShips(), _ownMap, _enemyMap);
        }

        #region private functions
        private void FillMaps()
        {
            int index = CommonVariables.FirstIndexOfX_Y_Axis;
            foreach (char y in CommonVariables.DefaultYAxis)
            {
                foreach (int x in CommonVariables.DefaultXAxis)
                {
                    _ownMap[index] = ExtraPoint.CreatePoint(Point.CreatePoint(x, y));
                    _enemyMap[index] = ExtraPoint.CreatePoint(Point.CreatePoint(x, y));
                    index++;
                }
            }
        }

        private List<Ship> CreateShips()
        {
            List<Ship> ships = new List<Ship>();
            foreach (var shipName in Enum.GetNames(typeof(ShipType)))
            {
                try
                {
                    ShipType shipType = (ShipType)Enum.Parse(typeof(ShipType), shipName);
                    ICollection<ExtraPoint> shipPoints = CreateShipPoints((int)shipType);
                    Ship ship = Ship.CreateShip(shipName, shipPoints);

                    ships.Add(ship);
                    ActualizeMap(shipPoints);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return ships;
        }

        private ICollection<ExtraPoint> CreateShipPoints(int shipLength)
        {
            ICollection<ExtraPoint>? shipPoints;
            CreateRandomPoint(out int xIndex, out int yIndex);

            int x = CommonVariables.DefaultXAxis[xIndex];
            char y = CommonVariables.DefaultYAxis[yIndex];

            ExtraPoint startPoint = ExtraPoint.CreatePoint(Point.CreatePoint(x, y), PointStatus.Taken);

            if (_ownMap.Single(p => p.Point.X == startPoint.Point.X && p.Point.Y == startPoint.Point.Y).Status == PointStatus.Free)
            {
                shipPoints = FindOtherPointsPart1(startPoint, shipLength);
                if (shipPoints == null)
                {
                    _guard = false;
                    shipPoints = CreateShipPoints(shipLength);
                }
            }
            else
            {
                shipPoints = CreateShipPoints(shipLength);
            }

            return shipPoints;
        }

        private void CreateRandomPoint(out int xIndex, out int yIndex)
        {
            xIndex = _random.Next(CommonVariables.DefaultXAxis.Length);
            yIndex = _random.Next(CommonVariables.DefaultYAxis.Length);
        }

        private ICollection<ExtraPoint>? FindOtherPointsPart1(ExtraPoint startPoint, int shipLength, int? arrangementChanged = null)
        {
            int arrangement;

            if (arrangementChanged == null)
            {
                arrangement = _random.Next(CommonVariables.Arrangement.Count());
            }
            else
            {
                arrangement = arrangementChanged.Value;
            }

            if (arrangement == CommonVariables.Horizontal)
            {
                return FindOtherPointsPart2(startPoint, shipLength, arrangement, null, startPoint.Point.Y);
            }
            else // Vertical arrangement
            {
                return FindOtherPointsPart2(startPoint, shipLength, arrangement, startPoint.Point.X);
            }
        }

        private ICollection<ExtraPoint>? FindOtherPointsPart2(ExtraPoint startPoint, int shipLength, int arrangement, int? x = null, char? y = null)
        {
            bool breakLoop = false;
            int minimalIndex, maximalIndex, index;
            int shipLengthCopy = shipLength;
            IndexType indexType;
            ICollection<int> indexes;
            ICollection<ExtraPoint>? points = new List<ExtraPoint>();

            if (x == null && y != null)
            {
                index = minimalIndex = maximalIndex = Array.IndexOf(CommonVariables.DefaultXAxis, startPoint.Point.X);
            }
            else
            {
                index = minimalIndex = maximalIndex = Array.IndexOf(CommonVariables.DefaultYAxis, startPoint.Point.Y);
            }

            indexes = new List<int>
                {
                    minimalIndex
                };

            points.Add(startPoint);
            shipLengthCopy--;

            while (shipLengthCopy > 0)
            {
                indexType = CheckIndexes(indexes);

                switch (indexType)
                {
                    case IndexType.Lower:
                        Decrement(minimalIndex, out minimalIndex, out breakLoop, x, y);
                        index = minimalIndex;
                        break;
                    case IndexType.Higher:
                        Increment(maximalIndex, out maximalIndex, out breakLoop, x, y);
                        index = maximalIndex;
                        break;
                    case IndexType.Default:
                        int previousOrNext = _random.Next(CommonVariables.Order.Count());
                        if (previousOrNext == CommonVariables.Previous)
                        {
                            Decrement(minimalIndex, out minimalIndex, out breakLoop, x, y);
                            index = minimalIndex;
                            if (breakLoop)
                            {
                                Increment(maximalIndex, out maximalIndex, out breakLoop, x, y);
                                index = maximalIndex;
                            }

                            indexes.Add(index);
                        }
                        else if (previousOrNext == CommonVariables.Next)
                        {
                            Increment(maximalIndex, out maximalIndex, out breakLoop, x, y);
                            index = maximalIndex;
                            if (breakLoop)
                            {
                                Decrement(minimalIndex, out minimalIndex, out breakLoop, x, y);
                                index = minimalIndex;
                            }
                            indexes.Add(index);
                        }
                        break;
                }

                if (breakLoop)
                {
                    if (_guard == true)
                    {
                        points = null;
                    }
                    else
                    {
                        _guard = true;
                        arrangement = arrangement == CommonVariables.Horizontal ? CommonVariables.Vertical : CommonVariables.Horizontal;
                        points = FindOtherPointsPart1(startPoint, shipLength, arrangement);
                    }
                    break;
                }
                else
                {
                    if (arrangement == CommonVariables.Horizontal)
                    {
                        points.Add(ExtraPoint.CreatePoint(Point.CreatePoint(CommonObjects.CommonVariables.DefaultXAxis[index], startPoint.Point.Y), PointStatus.Taken));

                    }
                    else
                    {
                        points.Add(ExtraPoint.CreatePoint(Point.CreatePoint(startPoint.Point.X, CommonObjects.CommonVariables.DefaultYAxis[index]), PointStatus.Taken));
                    }
                }

                shipLengthCopy--;
            }
            return points;
        }

        private void Increment(int index, out int indexIncremented, out bool breakLoop, int? x = null, char? y = null)
        {
            index++;
            indexIncremented = index;
            breakLoop = CheckIfBreakLoopNeeded(indexIncremented, x, y);
        }

        private void Decrement(int index, out int indexDecremented, out bool breakLoop, int? x = null, char? y = null)
        {
            index--;
            indexDecremented = index;
            breakLoop = CheckIfBreakLoopNeeded(indexDecremented, x, y);
        }

        private bool CheckIfBreakLoopNeeded(int index, int? x = null, char? y = null)
        {
            if (index < CommonVariables.FirstIndexOfX_Y_Axis || index > CommonVariables.LastIndexOfX_Y_Axis)
            {
                return true;
            }

            if (x == null && y != null)
            {
                return CheckStatus(CommonVariables.DefaultXAxis[index], y.Value);
            }
            else
            {
                if (x != null)
                {
                    return CheckStatus(x.Value, CommonVariables.DefaultYAxis[index]);
                }
                else
                {
                    return true;
                }
            }
        }

        private bool CheckStatus(int x, char y)
        {
            return !(_ownMap.Single(p => p.Point.X == x && p.Point.Y == y).Status == PointStatus.Free);
        }

        private IndexType CheckIndexes(IEnumerable<int> indexes)
        {
            IndexType indexType = IndexType.Default;
            for (int i = 0; i < indexes.Count(); i++)
            {
                int element = indexes.ElementAt(i);
                if (element == CommonVariables.FirstIndexOfX_Y_Axis)
                {
                    return IndexType.Higher;
                }
                else if (element == CommonVariables.LastIndexOfX_Y_Axis)
                {
                    return IndexType.Lower;
                }
            }
            return indexType;
        }

        private void ActualizeMap(IEnumerable<ExtraPoint> shipPoints)
        {
            foreach (ExtraPoint point in shipPoints)
            {
                ExtraPoint tmp = _ownMap.Single(p => p.Point.Y == point.Point.Y && p.Point.X == point.Point.X);
                int index = Array.IndexOf(_ownMap, tmp);
                _ownMap[index].Status = PointStatus.Taken;
            }
        }
        #endregion
    }
}
