using CommonObjects;
using System;

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

        public string NickName { get; set; }
        public List<Ship> Ships { get; set; }
        public Point[] OwnMap { get; set; }
        public Point[] EnemyMap { get; set; }


        private Player(string nickName, List<Ship> ships, Point[] ownMap, Point[] enemyMap)
        {
            NickName = nickName;
            Ships = ships;
            OwnMap = ownMap;
            EnemyMap = enemyMap;
        }

        public static Player CreatePlayer(string nickName, int mapSize = CommonObjects.CommonVariables.DefaultMapSize)
        {
            Point[] ownMap = new Point[mapSize];
            Point[] enemyMap = new Point[mapSize];

            FillMaps(ownMap, enemyMap, out ownMap, out enemyMap);

            CreateShips(new List<Ship>(), ownMap, out List<Ship> ships, out ownMap);


            return new Player(nickName, ships, ownMap, enemyMap);
        }

        private static void FillMaps(Point[] ownMap, Point[] enemyMap, out Point[] filledOwnMap, out Point[] filledEnemyMap)
        {
            int index = 0;
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
            foreach (var shipType in Enum.GetValues(typeof(ShipType)).Cast<ShipType>())
            {
                ICollection<Point> shipPoints = CreateShipPoints(ownMap, (int)shipType);
                Ship ship = Ship.CreateShip(shipType.ToString(), shipType, shipPoints.ToList());

                ships.Add(ship);
                ownMap = ActualizeMap(ownMap, shipPoints);
            }
            actualizedMap = ownMap;
            shipsFilled = ships;
        }

        private static ICollection<Point> CreateShipPoints(Point[] ownMap, int shipLength)
        {
            ICollection<Point>? shipPoints;
            CreateRandomPoint(out int xIndex, out int yIndex);

            int x = CommonVariables.DefaultXAxis[xIndex];
            char y = CommonVariables.DefaultYAxis[yIndex];

            Point startPoint = Point.CreatePoint(x, y, PointStatus.Taken);

            if (ownMap.Single(p => p.X == startPoint.X && p.Y == startPoint.Y).Status == PointStatus.Free)
            {
                shipPoints = FindOtherPoints(startPoint, shipLength, ownMap);
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

        private static void CreateRandomPoint(out int xIndex, out int yIndex, int firstIndex = CommonVariables.FirstIndexOfX_Y_Axis, int lastIndex = CommonVariables.LastIndexOfX_Y_Axis)
        {
            xIndex = _random.Next(firstIndex, lastIndex);
            yIndex = _random.Next(firstIndex, lastIndex);
        }

        private static ICollection<Point>? FindOtherPoints(Point startPoint, int shipLength, Point[] ownMap, int? hOv = null)
        {
            int horrizontallyOrVertically;
            if (hOv == null)
            {
                horrizontallyOrVertically = _random.Next(0, 1);
            }
            else
            {
                horrizontallyOrVertically = hOv.Value;
            }

            int minimalIndex, maximalIndex, index = 0;
            ICollection<int> indexes;
            ICollection<Point>? points = new List<Point>();
            IndexType indexType;
            bool breakLoop = false;

            points.Add(startPoint);
            shipLength--;

            if (horrizontallyOrVertically == 0) // horrizontally 
            {
                minimalIndex = maximalIndex = Array.IndexOf(CommonVariables.DefaultXAxis, startPoint.X);
                indexes = new List<int>();
                indexes.Add(minimalIndex);
                indexType = CheckIndex(indexes);

                while (shipLength > 0)
                {
                    switch (indexType)
                    {
                        case IndexType.Lower:
                            minimalIndex--;
                            index = minimalIndex;
                            breakLoop = CheckStatus(CommonVariables.DefaultXAxis[minimalIndex], startPoint.Y, ownMap);
                            break;
                        case IndexType.Higher:
                            maximalIndex++;
                            index = maximalIndex;
                            breakLoop = CheckStatus(CommonVariables.DefaultXAxis[maximalIndex], startPoint.Y, ownMap);
                            break;
                        case IndexType.Default:
                            int prev_next = _random.Next(0, 1);
                            if (prev_next == 0)
                            {
                                minimalIndex--;
                                index = minimalIndex;
                                breakLoop = CheckStatus(CommonVariables.DefaultXAxis[minimalIndex], startPoint.Y, ownMap);
                                if (breakLoop)
                                {
                                    maximalIndex++;
                                    index = maximalIndex;
                                    indexType = IndexType.Higher;
                                    breakLoop = CheckStatus(CommonVariables.DefaultXAxis[maximalIndex], startPoint.Y, ownMap);
                                }

                                indexes.Add(index);
                            }
                            else if (prev_next == 1)
                            {
                                maximalIndex++;
                                index = maximalIndex;
                                breakLoop = CheckStatus(CommonVariables.DefaultXAxis[maximalIndex], startPoint.Y, ownMap);
                                if (breakLoop)
                                {
                                    minimalIndex--;
                                    index = minimalIndex;
                                    indexType = IndexType.Lower;
                                    breakLoop = CheckStatus(CommonVariables.DefaultXAxis[minimalIndex], startPoint.Y, ownMap);
                                }
                                indexes.Add(index);
                            }

                            if (indexType == IndexType.Default)
                            {
                                indexType = CheckIndex(indexes);
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
                            points = FindOtherPoints(startPoint, shipLength, ownMap, 1);
                        }
                        break;
                    }
                    else
                    {
                        points.Add(Point.CreatePoint(CommonObjects.CommonVariables.DefaultXAxis[index], startPoint.Y, PointStatus.Taken));
                    }

                    shipLength--;
                }
                return points;
            }
            else // vertically
            {
                minimalIndex = maximalIndex = Array.IndexOf(CommonVariables.DefaultXAxis, startPoint.X);
                indexes = new List<int>();
                indexes.Add(minimalIndex);
                indexType = CheckIndex(indexes);

                while (shipLength > 0)
                {
                    switch (indexType)
                    {
                        case IndexType.Lower:
                            minimalIndex--;
                            index = minimalIndex;
                            breakLoop = CheckStatus(startPoint.X, CommonVariables.DefaultYAxis[minimalIndex], ownMap);
                            break;
                        case IndexType.Higher:
                            maximalIndex++;
                            index = maximalIndex;
                            breakLoop = CheckStatus(startPoint.X, CommonVariables.DefaultYAxis[maximalIndex], ownMap);
                            break;
                        case IndexType.Default:
                            int prev_next = _random.Next(0, 1);
                            if (prev_next == 0)
                            {
                                minimalIndex--;
                                index = minimalIndex;
                                breakLoop = CheckStatus(startPoint.X, CommonVariables.DefaultYAxis[minimalIndex], ownMap);
                                if (breakLoop)
                                {
                                    maximalIndex++;
                                    index = maximalIndex;
                                    indexType = IndexType.Higher;
                                    breakLoop = CheckStatus(startPoint.X, CommonVariables.DefaultYAxis[maximalIndex], ownMap);
                                }

                                indexes.Add(index);
                            }
                            else if (prev_next == 1)
                            {
                                maximalIndex++;
                                index = maximalIndex;
                                breakLoop = CheckStatus(startPoint.X, CommonVariables.DefaultYAxis[minimalIndex], ownMap);
                                if (breakLoop)
                                {
                                    minimalIndex--;
                                    index = minimalIndex;
                                    indexType = IndexType.Lower;
                                    breakLoop = CheckStatus(startPoint.X, CommonVariables.DefaultYAxis[maximalIndex], ownMap);
                                }
                                indexes.Add(index);
                            }

                            if (indexType == IndexType.Default)
                            {
                                indexType = CheckIndex(indexes);
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
                            points = FindOtherPoints(startPoint, shipLength, ownMap, 0);
                        }
                        break;
                    }
                    else
                    {
                        points.Add(Point.CreatePoint(CommonObjects.CommonVariables.DefaultXAxis[index], startPoint.Y, PointStatus.Taken));
                    }

                    shipLength--;
                }
                return points;
            }
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

        private static IndexType CheckIndex(IEnumerable<int> indexes)
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
    }
}
