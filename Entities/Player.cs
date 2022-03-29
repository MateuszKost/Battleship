using CommonObjects;

namespace MainObjects
{
    /*
     * Player - class for storing information about player
     * 
     * NickName - name of the player
     * Ships - the list of ships assigned to the player
     * OwnMap - Array with points which is representing own board like in board game
     * EnemyMap - Array with points which is representing enemy board like in board game
     */
    public class Player
    {
        private static readonly Random _random = new Random();
        private static bool _guard = false;

        public string NickName { get; init; }
        public List<Ship> Ships { get; init; }
        public bool HittedButNotSunk { get; set; }
        public Point[] OwnMap { get; set; }
        public Point[] EnemyMap { get; set; }

        private Player(string nickName, List<Ship> ships, Point[] ownMap, Point[] enemyMap)
        {
            NickName = nickName;
            Ships = ships;
            OwnMap = ownMap;
            EnemyMap = enemyMap;
            HittedButNotSunk = false;
        }

        public static Player CreatePlayer(string nickName, int mapSize = CommonVariables.DefaultMapSize)
        {
            Point[] ownMap = new Point[mapSize];
            Point[] enemyMap = new Point[mapSize];

            FillMaps(ownMap, enemyMap, out ownMap, out enemyMap);

            CreateShips(new List<Ship>(), ownMap, out List<Ship> ships, out ownMap);

            return new Player(nickName, ships, ownMap, enemyMap);
        }

        public PointStatus Shot(ValueTuple<int, char> point, IEnumerable<Ship> ships)
        {
            Console.WriteLine("Oddano strzał w {0}{1}", point.Item1, point.Item2);
            bool hitted = ships.SelectMany(s => s.Points).SingleOrDefault(p => p.X == point.Item1 && p.Y == point.Item2) != null;
            if (hitted)
            {
                Console.WriteLine("Trafiono");
                return PointStatus.Hitted;
            }
            else
            {
                Console.WriteLine("Pudlo");
                return PointStatus.Missed;
            }
        }

        public void UpdateEnemyMap(ValueTuple<int, char> pointValues, PointStatus pointStatus)
        {
            Point point = EnemyMap.Single(p => p.X == pointValues.Item1 && p.Y == pointValues.Item2);
            int index = Array.IndexOf(EnemyMap, point);
            EnemyMap[index].Status = pointStatus;
        }

        public PointStatus UpdateOwnMap(ValueTuple<int, char> pointValues)
        {
            Point point = EnemyMap.Single(p => p.X == pointValues.Item1 && p.Y == pointValues.Item2);
            int index = Array.IndexOf(EnemyMap, point);
            OwnMap[index].Status = PointStatus.Hitted;

            foreach (Ship ship in Ships)
            {
                point = ship.Points.SingleOrDefault(p => p.X == pointValues.Item1 && p.Y == pointValues.Item2);
                if (point != null)
                {
                    ship.Points.Remove(point);
                    HittedButNotSunk = true;
                    if (ship.Points.Count == 0)
                    {
                        Console.WriteLine("{0} gracza {1} został zatopiony", ship.ShipName, NickName);
                        HittedButNotSunk = false;
                        Ships.Remove(ship);

                        if (Ships.Count == 0)
                        {
                            Console.WriteLine("Gracza {0} przegrał gre", NickName);
                        }
                        return PointStatus.Sunk;
                    }
                }
            }
            return PointStatus.Hitted;
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

        private static void CreateShips(List<Ship> ships, Point[] ownMap, out List<Ship> shipsFilled, out Point[] actualizedMap)
        {
            foreach (var shipName in Enum.GetNames(typeof(ShipType)))
            {
                ShipType shipType = (ShipType)Enum.Parse(typeof(ShipType), shipName);
                IEnumerable<Point> shipPoints = CreateShipPoints(ownMap, (int)shipType);
                Ship ship = Ship.CreateShip(shipName, (int)shipType, shipPoints.ToList());

                ships.Add(ship);
                ownMap = ActualizeMap(ownMap, shipPoints);
            }
            actualizedMap = ownMap;
            shipsFilled = ships;
        }

        private static IEnumerable<Point> CreateShipPoints(Point[] ownMap, int shipLength)
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

        private static void CreateRandomPoint(out int xIndex, out int yIndex)
        {
            xIndex = _random.Next(CommonVariables.DefaultXAxis.Length);
            yIndex = _random.Next(CommonVariables.DefaultYAxis.Length);
        }

        private static ICollection<Point>? FindOtherPointsPart1(Point startPoint, int shipLength, Point[] ownMap, int? arrangementChanged = null)
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

        private static ICollection<Point>? FindOtherPointsPart2(Point startPoint, int shipLength, Point[] ownMap, int arrangement, int? x = null, char? y = null)
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

        private static void Increment(int index, Point[] ownMap, out int indexIncremented, out bool breakLoop, int? x = null, char? y = null)
        {
            index++;
            indexIncremented = index;
            breakLoop = CheckIfBreakLoopNeeded(indexIncremented, ownMap, x, y);
        }

        private static void Decrement(int index, Point[] ownMap, out int indexDecremented, out bool breakLoop, int? x = null, char? y = null)
        {
            index--;
            indexDecremented = index;
            breakLoop = CheckIfBreakLoopNeeded(indexDecremented, ownMap, x, y);
        }

        private static bool CheckIfBreakLoopNeeded(int index, Point[] ownMap, int? x = null, char? y = null)
        {
            return x == null && y != null ? CheckStatus(CommonVariables.DefaultXAxis[index], y.Value, ownMap) : CheckStatus(x.Value, CommonVariables.DefaultYAxis[index], ownMap); // index = -1 problem
        }

        private static bool CheckStatus(int x, char y, Point[] ownMap)
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

        private static IndexType CheckIndexes(IEnumerable<int> indexes)
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

        private static Point[] ActualizeMap(Point[] ownMap, IEnumerable<Point> shipPoints)
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
