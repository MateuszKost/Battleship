using Battleship;
using MainObjects;
using Microsoft.AspNetCore.Mvc;
using ViewModels;

namespace BattlehipGame.Server.Controllers
{
    public class GameController : Controller
    {
        [HttpGet("GetPlayers")]
        public PlayerViewModel[] GetPlayers()
        {
            CreationAlgorithm creationAlgorithm = new CreationAlgorithm();
            Player firstPlayer =  creationAlgorithm.CreatePlayer("First player");
            Player secondPlayer =  creationAlgorithm.CreatePlayer("Second player");

            return new PlayerViewModel[] { new PlayerViewModel(firstPlayer.NickName, firstPlayer.OwnMap), new PlayerViewModel(secondPlayer.NickName, secondPlayer.OwnMap) };
        }

        [HttpGet("GetSimulationList")]
        public IEnumerable<ShootViewModel> GetSimulationList()
        {
            CreationAlgorithm creationAlgorithm = new CreationAlgorithm();
            //Check later ship creating, some points of ship sometimes are not in line
            Player playerOne = creationAlgorithm.CreatePlayer("First player");
            Console.WriteLine("Created {0}", playerOne.NickName);
            Player playerTwo = creationAlgorithm.CreatePlayer("Second player");
            Console.WriteLine("Created {0}", playerTwo.NickName);
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Starting the simulation...");

            SimulationAlgorithm simulation = new SimulationAlgorithm();
            simulation.Start(playerOne, playerTwo);

            return simulation.Start(playerOne, playerTwo);
        }
    }
}
