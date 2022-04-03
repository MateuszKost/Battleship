using ViewModels;

namespace BattlehipGame.Client.Pages
{
    public partial class Game
    {
        PlayerViewModel[]? players;
        IEnumerable<ShootViewModel>? simulationList;

        protected override async Task OnInitializedAsync()
        {
            
        }

        private async Task GetPlayers()
        {
            players = await GameService.GetPlayers();
        }

        private async Task Simulate()
        {
            simulationList = await GameService.GetSimulationList();
        }
    }
}
