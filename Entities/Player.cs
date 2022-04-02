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
        public string NickName { get; init; }
        public ICollection<Ship> Ships { get; init; }
        public ExtraPoint[] OwnMap { get; set; }
        private ExtraPoint[] EnemyMap { get; set; }

        private Player(string nickName, ICollection<Ship> ships, ExtraPoint[] ownMap, ExtraPoint[] enemyMap)
        {
            NickName = nickName;
            Ships = ships;
            OwnMap = ownMap;
            EnemyMap = enemyMap;
        }

        public static Player CreatePlayer(string nickName, ICollection<Ship> ships, ExtraPoint[] ownMap, ExtraPoint[] enemyMap)
        {
            return new Player(nickName, ships, ownMap, enemyMap);
        }

        public PointStatus Shot(Point point, ICollection<Ship> ships)
        {
            Console.WriteLine(CommonVariables.ShootedPointInPlace, point.X, point.Y);
            bool hit = ships.SelectMany(s => s.Points).Select(p => p.Point).Contains(point);
            if (hit)
            {
                Console.WriteLine(CommonVariables.Hit);
                return PointStatus.Hit;
            }
            else
            {
                Console.WriteLine(CommonVariables.Missed);
                return PointStatus.Missed;
            }
        }

        public void UpdateEnemyMap(Point pointValues, PointStatus pointStatus)
        {
            ExtraPoint point = EnemyMap.Single(p => p.Point.X == pointValues.X && p.Point.Y == pointValues.Y);
            int index = Array.IndexOf(EnemyMap, point);
            EnemyMap[index].Status = pointStatus;
        }

        public PointStatus UpdateOwnMap(Point pointValues)
        {
            ExtraPoint point = OwnMap.Single(p => p.Point.X == pointValues.X && p.Point.Y == pointValues.Y);
            int index = Array.IndexOf(OwnMap, point);
            OwnMap[index].Status = PointStatus.Hit;

            foreach (Ship ship in Ships)
            {
                if(ship.Points.Contains(point))
                {
                    ship.Points.Remove(point);
                    if (ship.Points.Count == 0)
                    {
                        Console.WriteLine(CommonVariables.ShipWithNumberSunk, ship.ShipName, NickName);
                        Ships.Remove(ship);

                        if (Ships.Count == 0)
                        {
                            Console.WriteLine(CommonVariables.PlayerWithNumberLost, NickName);
                        }
                        return PointStatus.Sunk;
                    }
                }
            }
            return PointStatus.Hit;
        }
    }
}
