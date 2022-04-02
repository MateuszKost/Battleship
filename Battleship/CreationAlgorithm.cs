using CommonObjects;
using MainObjects;

namespace Battleship
{
    public class CreationAlgorithm
    {
        private readonly Random _random = new Random();
        private bool _guard = false;

        public Player CreatePlayer(string firstPlayerName)
        {
            ExtraPoint[] ownMap = new ExtraPoint[CommonVariables.DefaultMapSize];
            ExtraPoint[] enemyMap = new ExtraPoint[CommonVariables.DefaultMapSize];

            FillMaps(ownMap, enemyMap, out ownMap, out enemyMap);

            CreateShips(new List<Ship>(), ownMap, out List<Ship> ships, out ownMap);

            return Player.CreatePlayer(firstPlayerName, ships, ownMap, enemyMap);
        }

        #region private functions
        private static void FillMaps(ExtraPoint[] ownMap, ExtraPoint[] enemyMap, out ExtraPoint[] filledOwnMap, out ExtraPoint[] filledEnemyMap)
        {
            int index = CommonVariables.FirstIndexOfX_Y_Axis;
            foreach (char y in CommonVariables.DefaultYAxis)
            {
                foreach (int x in CommonVariables.DefaultXAxis)
                {
                    ownMap[index] = ExtraPoint.CreatePoint(Point.CreatePoint(x, y));
                    enemyMap[index] = ExtraPoint.CreatePoint(Point.CreatePoint(x, y));
                    index++;
                }
            }
            filledOwnMap = ownMap;
            filledEnemyMap = enemyMap;
        }

        private void CreateShips(List<Ship> ships, ExtraPoint[] ownMap, out List<Ship> shipsFilled, out ExtraPoint[] actualizedMap)
        {
            foreach (var shipName in Enum.GetNames(typeof(ShipType)))
            {
                try
                {
                    ShipType shipType = (ShipType)Enum.Parse(typeof(ShipType), shipName);
                    IEnumerable<ExtraPoint> shipPoints = CreateShipPoints(ownMap, (int)shipType);
                    Ship ship = Ship.CreateShip(shipName, shipPoints.ToList());

                    ships.Add(ship);
                    ownMap = ActualizeMap(ownMap, shipPoints);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            actualizedMap = ownMap;
            shipsFilled = ships;
        }

        private IEnumerable<ExtraPoint> CreateShipPoints(ExtraPoint[] ownMap, int shipLength)
        {
            IEnumerable<ExtraPoint>? shipPoints;
            CreateRandomPoint(out int xIndex, out int yIndex);

            int x = CommonVariables.DefaultXAxis[xIndex];
            char y = CommonVariables.DefaultYAxis[yIndex];

            ExtraPoint startPoint = ExtraPoint.CreatePoint(Point.CreatePoint(x, y), PointStatus.Taken);

            if (ownMap.Single(p => p.Point.X == startPoint.Point.X && p.Point.Y == startPoint.Point.Y).Status == PointStatus.Free)
            {
                shipPoints = FindOtherPointsPart1(startPoint, shipLength, ownMap);
                if (shipPoints == null)
                {
                    _guard = false;
                    shipPoints = CreateShipPoints(ownMap, shipLength);
                }
            }
            else
            {
                shipPoints = CreateShipPoints(ownMap, shipLength);
            }

            return shipPoints;
        }

        private void CreateRandomPoint(out int xIndex, out int yIndex)
        {
            xIndex = _random.Next(CommonVariables.DefaultXAxis.Length);
            yIndex = _random.Next(CommonVariables.DefaultYAxis.Length);
        }

        private ICollection<ExtraPoint>? FindOtherPointsPart1(ExtraPoint startPoint, int shipLength, ExtraPoint[] ownMap, int? arrangementChanged = null)
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
                return FindOtherPointsPart2(startPoint, shipLength, ownMap, arrangement, null, startPoint.Point.Y);
            }
            else // Vertical arrangement
            {
                return FindOtherPointsPart2(startPoint, shipLength, ownMap, arrangement, startPoint.Point.X);
            }
        }

        private ICollection<ExtraPoint>? FindOtherPointsPart2(ExtraPoint startPoint, int shipLength, ExtraPoint[] ownMap, int arrangement, int? x = null, char? y = null)
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
                        Decrement(minimalIndex, ownMap, out minimalIndex, out breakLoop, x, y);
                        index = minimalIndex;
                        break;
                    case IndexType.Higher:
                        Increment(maximalIndex, ownMap, out maximalIndex, out breakLoop, x, y);
                        index = maximalIndex;
                        break;
                    case IndexType.Default:
                        int previousOrNext = _random.Next(CommonVariables.Order.Count());
                        if (previousOrNext == CommonVariables.Previous)
                        {
                            Decrement(minimalIndex, ownMap, out minimalIndex, out breakLoop, x, y);
                            index = minimalIndex;
                            if (breakLoop)
                            {
                                Increment(maximalIndex, ownMap, out maximalIndex, out breakLoop, x, y);
                                index = maximalIndex;
                            }

                            indexes.Add(index);
                        }
                        else if (previousOrNext == CommonVariables.Next)
                        {
                            Increment(maximalIndex, ownMap, out maximalIndex, out breakLoop, x, y);
                            index = maximalIndex;
                            if (breakLoop)
                            {
                                Decrement(minimalIndex, ownMap, out minimalIndex, out breakLoop, x, y);
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
                        points = FindOtherPointsPart1(startPoint, shipLength, ownMap, arrangement);
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

        private void Increment(int index, ExtraPoint[] ownMap, out int indexIncremented, out bool breakLoop, int? x = null, char? y = null)
        {
            index++;
            indexIncremented = index;
            breakLoop = CheckIfBreakLoopNeeded(indexIncremented, ownMap, x, y);
        }

        private void Decrement(int index, ExtraPoint[] ownMap, out int indexDecremented, out bool breakLoop, int? x = null, char? y = null)
        {
            index--;
            indexDecremented = index;
            breakLoop = CheckIfBreakLoopNeeded(indexDecremented, ownMap, x, y);
        }

        private bool CheckIfBreakLoopNeeded(int index, ExtraPoint[] ownMap, int? x = null, char? y = null)
        {
            if (index < CommonVariables.FirstIndexOfX_Y_Axis || index > CommonVariables.LastIndexOfX_Y_Axis)
            {
                return true;
            }

            if (x == null && y != null)
            {
                return CheckStatus(CommonVariables.DefaultXAxis[index], y.Value, ownMap);
            }
            else
            {
                if (x != null)
                {
                    return CheckStatus(x.Value, CommonVariables.DefaultYAxis[index], ownMap);
                }
                else
                {
                    return true;
                }
            }
        }

        private bool CheckStatus(int x, char y, ExtraPoint[] ownMap)
        {
            if (ownMap.Single(p => p.Point.X == x && p.Point.Y == y).Status == PointStatus.Free)
            {
                return false;
            }
            else
            {
                return true;
            }
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

        private ExtraPoint[] ActualizeMap(ExtraPoint[] ownMap, IEnumerable<ExtraPoint> shipPoints)
        {
            //Should it work?
            foreach (ExtraPoint point in shipPoints)
            {
                ExtraPoint tmp = ownMap.Single(p => p.Point.Y == point.Point.Y && p.Point.X == point.Point.X);
                int index = Array.IndexOf(ownMap, tmp);
                //ownMap.Single(p => p.Point.X == point.Point.X && p.Point.Y == point.Point.Y).Status = PointStatus.Taken;
                ownMap[index].Status = PointStatus.Taken;
            }

            return ownMap;
        }
        #endregion
    }
}
