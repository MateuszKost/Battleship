using CommonObjects;
using Microsoft.JSInterop;
using ViewModels;

namespace BattlehipGame.Client.Pages
{
    public partial class Game
    {
        PlayerViewModel[]? players;
        IEnumerable<ShootViewModel>? simulationList;
        string _name = string.Empty;
        string _firstPlayerSquare = "firstPlayerSquare";
        string _secondPlayerSquare = "secondPlayerSquare";
        bool _valuesCreated = false;


        protected override async Task OnInitializedAsync()
        {

        }

        private async Task StartGame()
        {
            players = await GameService.GetPlayers();
            simulationList = await GameService.GetSimulationList();
            _valuesCreated = true;
            StateHasChanged();
        }

        protected async Task CallJSMethod(int x, char y, PointStatus pointStatus, bool playerTurn)
        {
            if(playerTurn)
            {
                switch(pointStatus)
                {
                    case PointStatus.Hit:
                        await Current.InvokeVoidAsync("changeColor", _firstPlayerSquare + x.ToString() + y, "red");
                        break;
                    case PointStatus.Missed:
                        await Current.InvokeVoidAsync("changeColor", _firstPlayerSquare + x.ToString() + y, "blue");
                        break;
                    case PointStatus.Taken:
                        await Current.InvokeVoidAsync("changeColor", _firstPlayerSquare + x.ToString() + y, "black");
                        break;
                    case PointStatus.Free:
                        break;
                }
            }
            else
            {
                switch (pointStatus)
                {
                    case PointStatus.Hit:
                        await Current.InvokeVoidAsync("changeColor", _secondPlayerSquare + x.ToString() + y, "red");
                        break;
                    case PointStatus.Missed:
                        await Current.InvokeVoidAsync("changeColor", _secondPlayerSquare + x.ToString() + y, "blue");
                        break;
                    case PointStatus.Taken:
                        await Current.InvokeVoidAsync("changeColor", _secondPlayerSquare + x.ToString() + y, "black");
                        break;
                    case PointStatus.Free:
                        break;
                }
            }
        }
    }
}
