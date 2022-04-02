using CommonObjects;

namespace MainObjects
{
    /*
     * Ship - class for storing information about ship
     * 
     * ShipName - name of ship
     * Points - list of points for ship, depends on the ship type it got length form 2 to 5  
     */
    public class Ship
    {
        public string ShipName { get; }
        public ICollection<ExtraPoint> Points { get; }

        private Ship(string shipName, ICollection<ExtraPoint> points)
        {
            ShipName = shipName;
            Points = points;
        }

        public static Ship CreateShip(string shipName, ICollection<ExtraPoint> points)
        {
            if (points == null)
            {
                throw new ArgumentNullException(nameof(points));
            }

            return new Ship(shipName, points);
        }
    }
}
