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

        public static Player CreatePlayer(string nickName, List<Ship>? ships = null, int mapSize = CommonObjects.CommonVariables.DefaultMapSize)
        {
            Point[] ownMap = new Point[mapSize];
            Point[] enemyMap = new Point[mapSize];

            FillMaps(ownMap, enemyMap, out ownMap, out enemyMap);

            if(ships == null)
            {
                ships = CreateShips(new List<Ship>(), ownMap);
            }

            return new Player(nickName, ships, ownMap, enemyMap);
        }

        private static void FillMaps(Point[] ownMap, Point[] enemyMap, out Point[] filledOwnMap, out Point[] filledEnemyMap)
        {
            int index = 0;
            foreach(char y in CommonVariables.DefaultYAxis)
            {
                foreach(int x in CommonVariables.DefaultXAxis)
                {
                    ownMap[index] = Point.CreatePoint(x, y);
                    enemyMap[index] = Point.CreatePoint(x, y);
                    index++;
                }
            }
            filledOwnMap = ownMap;
            filledEnemyMap = enemyMap;
        }

        private static List<Ship> CreateShips(List<Ship> ships, Point[] ownMap)
        {
            foreach(var shipType in Enum.GetValues(typeof(ShipType)).Cast<ShipType>())
            {
                List<Point> shipPoints = CreateShipPoints(ownMap, (int)shipType);
                Ship ship = Ship.CreateShip(shipType.ToString(), shipType, shipPoints);

                ships.Add(ship);
            }


            return ships;
        }

        private static List<Point> CreateShipPoints(Point[] ownMap, int shipLength)
        {
            CreateRandomPoint(out int xIndex, out int yIndex);

            int x = CommonVariables.DefaultXAxis[xIndex];
            char y = CommonVariables.DefaultYAxis[yIndex];

            if(ownMap.Single(p => p.X == x && p.Y == y).Status == PointStatus.Free)
            {

            }
            else
            {

            }

            return new List<Point>();
        }

        private static void CreateRandomPoint(out int xIndex, out int yIndex, int firstIndex = CommonVariables.FirstIndexOfX_Y_Axis, int lastIndex = CommonVariables.LastIndexOfX_Y_Axis)
        {
            Random random = new Random();
            xIndex = random.Next(firstIndex, lastIndex);
            yIndex = random.Next(firstIndex, lastIndex);
        }
    }
}
