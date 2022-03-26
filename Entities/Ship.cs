using CommonObjects;

namespace MainObjects
{
    /*
     * Ship - class for storing information about ship
     * 
     * ShipName - name of ship
     * IsSunken - value which is setted as false if it's not sunken, or true in opposite side
     * Points - list of points for ship, depends on the ship type it got length form 2 to 5  
     */
    public class Ship
    {
        public string ShipName { get; private set; }
        public ShipType ShipType { get; private set; }
        public bool IsSunk { get; set; } = false;
        public int ShipSize { get; private set; }
        public List<Point> Points { get; private set; }

        private Ship(string shipName, ShipType shipType, int shipSize, List<Point> points)
        {
            ShipName = shipName;
            ShipType = shipType;
            ShipSize = shipSize;
            Points = points;
        }

        public static Ship CreateShip(string shipName, ShipType shipType, List<Point>? points = null)
        {
            if(points == null)
            {
                points = new List<Point>();
            }

            return new Ship(shipName, shipType, (int)shipType, points);
        }
    }
}
