namespace CommonObjects
{
    public enum ShipType : int
    {
        Carrier = 5,
        Battleship = 4,
        Destroyer = 3,
        Submarine = Destroyer,
        PatrolBoat = 2
    }
}
