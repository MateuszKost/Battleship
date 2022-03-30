using CommonObjects;

namespace MainObjects
{
    /*
     * Ship - class for storing information about ship
     * 
     * ShipName - name of ship
     * ShipSize - size of ship
     * IsSunken - value which is setted as false if it's not sunken, or true in opposite side
     * Points - list of points for ship, depends on the ship type it got length form 2 to 5  
     */
    public class Ship
    {
        public string ShipName { get; init; }
        public int ShipSize { get; init; }
        public bool IsSunk { get; set; } = false;
        public List<Point> Points { get; init; }

        private Ship(string shipName, int shipSize, List<Point> points)
        {
            ShipName = shipName;
            ShipSize = shipSize;
            Points = points;
        }

        public static Ship CreateShip(string shipName, int shipSize, List<Point> points)
        {
            if (points == null)
            {
                throw new ArgumentNullException(nameof(points));
            }

            return new Ship(shipName, shipSize, points);
        }
    }
}
