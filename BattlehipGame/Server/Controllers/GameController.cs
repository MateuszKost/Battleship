using Battleship;
using MainObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using ViewModels;

namespace BattlehipGame.Server.Controllers
{
    public class GameController : Controller
    {
        private Player _firstPlayer;
        private Player _secondPlayer;
        private IMemoryCache _cache;

        public GameController(IMemoryCache cache)
        {
            _cache = cache;
        }

        [HttpGet("GetPlayers")]
        public PlayerViewModel[] GetPlayers()
        {
            CreationAlgorithm creationAlgorithm = new CreationAlgorithm();
            _firstPlayer =  creationAlgorithm.CreatePlayer("First player");
            _secondPlayer =  creationAlgorithm.CreatePlayer("Second player");

            _cache.Set("firstPlayer", _firstPlayer);
            _cache.Set("secondPlayer", _secondPlayer);

            return new PlayerViewModel[] { new PlayerViewModel(_firstPlayer.NickName, _firstPlayer.OwnMap), new PlayerViewModel(_secondPlayer.NickName, _secondPlayer.OwnMap) };
        }

        [HttpGet("GetSimulationList")]
        public IEnumerable<ShootViewModel> GetSimulationList()
        {
            _firstPlayer = _cache.Get<Player>("firstPlayer");
            _secondPlayer = _cache.Get<Player>("secondPlayer");
            //Check later ship creating, some points of ship sometimes are not in line
            SimulationAlgorithm simulation = new SimulationAlgorithm();
            return simulation.Start(_firstPlayer, _secondPlayer);
        }
    }
}
