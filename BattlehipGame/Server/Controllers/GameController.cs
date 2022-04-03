using Battleship;
using MainObjects;
using Microsoft.AspNetCore.Mvc;
using ViewModels;

namespace BattlehipGame.Server.Controllers
{
    public class GameController : Controller
    {
        private Dictionary<Player, bool> _temp = new Dictionary<Player, bool>();

        private Player _firstPlayer;
        private Player _secondPlayer;

        [HttpGet("GetPlayers")]
        public PlayerViewModel[] GetPlayers()
        {
            CreationAlgorithm creationAlgorithm = new CreationAlgorithm();
            _firstPlayer =  creationAlgorithm.CreatePlayer("First player");
            _secondPlayer =  creationAlgorithm.CreatePlayer("Second player");

            _temp.Add(_firstPlayer, true);
            _temp.Add(_secondPlayer, true);

            return new PlayerViewModel[] { new PlayerViewModel(_firstPlayer.NickName, _firstPlayer.OwnMap), new PlayerViewModel(_secondPlayer.NickName, _secondPlayer.OwnMap) };
        }

        [HttpGet("GetSimulationList")]
        public IEnumerable<ShootViewModel> GetSimulationList()
        {
            CreationAlgorithm creationAlgorithm = new CreationAlgorithm();
            _firstPlayer = creationAlgorithm.CreatePlayer("First player");
            _secondPlayer = creationAlgorithm.CreatePlayer("Second player");
            //Check later ship creating, some points of ship sometimes are not in line
            Console.WriteLine("Created {0}", _firstPlayer.NickName);
            Console.WriteLine("Created {0}", _secondPlayer.NickName);
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Starting the simulation...");

            SimulationAlgorithm simulation = new SimulationAlgorithm();

            return simulation.Start(_firstPlayer, _secondPlayer);
        }
    }
}
