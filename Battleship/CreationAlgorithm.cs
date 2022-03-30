using CommonObjects;
using MainObjects;

namespace Battleship
{
    internal class CreationAlgorithm
    {
        private readonly Random _random = new Random();
        private bool _guard = false;

        internal Player CreatePlayer(string firstPlayerName)
        {
            Point[] ownMap = new Point[CommonVariables.DefaultMapSize];
            Point[] enemyMap = new Point[CommonVariables.DefaultMapSize];

            FillMaps(ownMap, enemyMap, out ownMap, out enemyMap);

            CreateShips(new List<Ship>(), ownMap, out List<Ship> ships, out ownMap);

            return Player.CreatePlayer(firstPlayerName, ships, ownMap, enemyMap);
        }

        #region private functions
        private static void FillMaps(Point[] ownMap, Point[] enemyMap, out Point[] filledOwnMap, out Point[] filledEnemyMap)
        {
            int index = CommonVariables.FirstIndexOfX_Y_Axis;
            foreach (char y in CommonVariables.DefaultYAxis)
            {
                foreach (int x in CommonVariables.DefaultXAxis)
                {
                    ownMap[index] = Point.CreatePoint(x, y);
                    enemyMap[index] = Point.CreatePoint(x, y);
                    index++;
                }
            }
            filledOwnMap = ownMap;
            filledEnemyMap = enemyMap;
        }

        private void CreateShips(List<Ship> ships, Point[] ownMap, out List<Ship> shipsFilled, out Point[] actualizedMap)
        {
            foreach (var shipName in Enum.GetNames(typeof(ShipType)))
            {
                try
                {
                    ShipType shipType = (ShipType)Enum.Parse(typeof(ShipType), shipName);
                    IEnumerable<Point> shipPoints = CreateShipPoints(ownMap, (int)shipType);
                    Ship ship = Ship.CreateShip(shipName, (int)shipType, shipPoints.ToList());

                    ships.Add(ship);
                    ownMap = ActualizeMap(ownMap, shipPoints);
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            actualizedMap = ownMap;
            shipsFilled = ships;
        }

        private IEnumerable<Point> CreateShipPoints(Point[] ownMap, int shipLength)
        {
            IEnumerable<Point>? shipPoints;
            CreateRandomPoint(out int xIndex, out int yIndex);

            int x = CommonVariables.DefaultXAxis[xIndex];
            char y = CommonVariables.DefaultYAxis[yIndex];

            Point startPoint = Point.CreatePoint(x, y, PointStatus.Taken);

            if (ownMap.Single(p => p.X == startPoint.X && p.Y == startPoint.Y).Status == PointStatus.Free)
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

        private ICollection<Point>? FindOtherPointsPart1(Point startPoint, int shipLength, Point[] ownMap, int? arrangementChanged = null)
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
                return FindOtherPointsPart2(startPoint, shipLength, ownMap, arrangement, null, startPoint.Y);
            }
            else // Vertical arrangement
            {
                return FindOtherPointsPart2(startPoint, shipLength, ownMap, arrangement, startPoint.X);
            }
        }

        private ICollection<Point>? FindOtherPointsPart2(Point startPoint, int shipLength, Point[] ownMap, int arrangement, int? x = null, char? y = null)
        {
            bool breakLoop = false;
            int minimalIndex, maximalIndex, index;
            int shipLengthCopy = shipLength;
            IndexType indexType;
            ICollection<int> indexes;
            ICollection<Point>? points = new List<Point>();

            if (x == null && y != null)
            {
                index = minimalIndex = maximalIndex = Array.IndexOf(CommonVariables.DefaultXAxis, startPoint.X);
            }
            else
            {
                index = minimalIndex = maximalIndex = Array.IndexOf(CommonVariables.DefaultYAxis, startPoint.Y);
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
                        points.Add(Point.CreatePoint(CommonObjects.CommonVariables.DefaultXAxis[index], startPoint.Y, PointStatus.Taken));

                    }
                    else
                    {
                        points.Add(Point.CreatePoint(startPoint.X, CommonObjects.CommonVariables.DefaultYAxis[index], PointStatus.Taken));
                    }
                }

                shipLengthCopy--;
            }
            return points;
        }

        private void Increment(int index, Point[] ownMap, out int indexIncremented, out bool breakLoop, int? x = null, char? y = null)
        {
            index++;
            indexIncremented = index;
            breakLoop = CheckIfBreakLoopNeeded(indexIncremented, ownMap, x, y);
        }

        private void Decrement(int index, Point[] ownMap, out int indexDecremented, out bool breakLoop, int? x = null, char? y = null)
        {
            index--;
            indexDecremented = index;
            breakLoop = CheckIfBreakLoopNeeded(indexDecremented, ownMap, x, y);
        }

        private bool CheckIfBreakLoopNeeded(int index, Point[] ownMap, int? x = null, char? y = null)
        {
            if(index < CommonVariables.FirstIndexOfX_Y_Axis || index > CommonVariables.LastIndexOfX_Y_Axis)
            {
                return true; 
            }
            return x == null && y != null ? CheckStatus(CommonVariables.DefaultXAxis[index], y.Value, ownMap) : CheckStatus(x.Value, CommonVariables.DefaultYAxis[index], ownMap);
        }

        private bool CheckStatus(int x, char y, Point[] ownMap)
        {
            if (ownMap.Single(p => p.X == x && p.Y == y).Status == PointStatus.Free)
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

        private Point[] ActualizeMap(Point[] ownMap, IEnumerable<Point> shipPoints)
        {
            foreach (Point point in shipPoints)
            {
                ownMap.Single(p => p.X == point.X && p.Y == point.Y).Status = PointStatus.Taken;
            }

            return ownMap;
        }
        #endregion
    }
}
