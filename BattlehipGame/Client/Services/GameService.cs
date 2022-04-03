using System.Net.Http.Json;
using ViewModels;

namespace BattlehipGame.Client.Services
{
    public class GameService
    {
        private HttpClient _httpClient;

        public GameService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<PlayerViewModel[]?> GetPlayers()
        {
            return await _httpClient.GetFromJsonAsync<PlayerViewModel[]>("GetPlayers"); ;
        }

        public async Task<IEnumerable<ShootViewModel>?> GetSimulationList()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<ShootViewModel>>("GetSimulationList");
        }
    }
}
