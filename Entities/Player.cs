using CommonObjects;

namespace MainObjects
{
    /*
     * Player - class for storing information about player
     * 
     * NickName - name of the player
     * Ships - the list of ships assigned to the player
     * HittedButNoSunk - informing us when our enemy hit us, but he didn't sunk our ship
     * OwnMap - Array with points which is representing own board like in board game
     * EnemyMap - Array with points which is representing enemy board like in board game
     */
    public class Player
    {
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

        public static Player CreatePlayer(string nickName, List<Ship> ships, Point[] ownMap, Point[] enemyMap)
        {
            return new Player(nickName, ships, ownMap, enemyMap);
        }

        public PointStatus Shot(ValueTuple<int, char> point, IEnumerable<Ship> ships)
        {
            Console.WriteLine(CommonVariables.ShootedPointInPlace, point.Item1, point.Item2);
            bool hitted = ships.SelectMany(s => s.Points).SingleOrDefault(p => p.X == point.Item1 && p.Y == point.Item2) != null;
            if (hitted)
            {
                Console.WriteLine(CommonVariables.Hitted);
                return PointStatus.Hitted;
            }
            else
            {
                Console.WriteLine(CommonVariables.Missed);
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
            Point? point = EnemyMap.Single(p => p.X == pointValues.Item1 && p.Y == pointValues.Item2);
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
                        Console.WriteLine(CommonVariables.ShipWithNumberSunk, ship.ShipName, NickName);
                        HittedButNotSunk = false;
                        Ships.Remove(ship);

                        if (Ships.Count == 0)
                        {
                            Console.WriteLine(CommonVariables.PlayerWithNumberLost, NickName);
                        }
                        return PointStatus.Sunk;
                    }
                }
            }
            return PointStatus.Hitted;
        }
    }
}
