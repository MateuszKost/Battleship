using Battleship;
using MainObjects;
using Microsoft.AspNetCore.Mvc;

namespace BattlehipGame.Server.Controllers
{
    public class GameController : Controller
    {
        [HttpGet("GetPlayers")]
        public ActionResult<Player[]> GetPlayers()
        {
            CreationAlgorithm creationAlgorithm = new CreationAlgorithm();
            Player firstPlayer =  creationAlgorithm.CreatePlayer("First player");
            Player secondPlayer =  creationAlgorithm.CreatePlayer("Second player");

            return new Player[] { firstPlayer, secondPlayer };
        }
    }
}
