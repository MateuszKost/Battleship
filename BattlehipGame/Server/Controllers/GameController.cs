using Battlehip.ViewModels;
using Battleship;
using MainObjects;
using Microsoft.AspNetCore.Mvc;

namespace BattlehipGame.Server.Controllers
{
    public class GameController : Controller
    {
        [HttpGet("GetPlayers")]
        public ActionResult<PlayerViewModel[]> GetPlayers()
        {
            CreationAlgorithm creationAlgorithm = new CreationAlgorithm();
            Player firstPlayer =  creationAlgorithm.CreatePlayer("First player");
            Player secondPlayer =  creationAlgorithm.CreatePlayer("Second player");

            return new PlayerViewModel[] { new PlayerViewModel(firstPlayer.NickName, firstPlayer.OwnMap), new PlayerViewModel(secondPlayer.NickName, secondPlayer.OwnMap) };
        }

        [HttpGet("GetSimulationList")]
        public ActionResult<IEnumerable<ShootViewModel>> GetSimulationList()
        {
            CreationAlgorithm creationAlgorithm = new CreationAlgorithm();
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
