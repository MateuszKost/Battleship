using MainObjects;

namespace Battleship
{
    internal static class Game
    {
        internal static void Start()
        {
            Creation creationAlgorithm = new Creation();
            Player playerOne = creationAlgorithm.CreatePlayer("First player");
            Console.WriteLine("Created {0}", playerOne.NickName);
            Player playerTwo = creationAlgorithm.CreatePlayer("Second player");
            Console.WriteLine("Created {0}", playerTwo.NickName);
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Starting the simulation...");

            Simulation simulation = new Simulation();
            simulation.Start(playerOne, playerTwo);
            Console.WriteLine("koniec gry");
        }
    }
}
