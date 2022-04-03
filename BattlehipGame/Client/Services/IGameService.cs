using ViewModels;

namespace BattlehipGame.Client.Services
{
    public interface IGameService
    {
        Task<PlayerViewModel[]?> GetPlayers();
        Task<IEnumerable<ShootViewModel>?> GetSimulationList();
    }
}