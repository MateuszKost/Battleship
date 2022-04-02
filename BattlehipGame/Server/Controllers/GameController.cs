using BattlehipGame.Server.ViewModels;
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
            //execute simulation function

            return new List<ShootViewModel> { new ShootViewModel(ExtraPoint.CreatePoint(Point.CreatePoint(1, 'a')), true), new ShootViewModel(ExtraPoint.CreatePoint(Point.CreatePoint(1, 'b')), true), new ShootViewModel(ExtraPoint.CreatePoint(Point.CreatePoint(2, 'c')), false) }; // to postman tests
        }
    }
}
