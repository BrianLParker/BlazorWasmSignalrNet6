using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.SignalR.Client;

namespace BlazorWasmSignalrNet6.Client.Pages;

public partial class ChatPage : IAsyncDisposable
{
    private HubConnection? hubConnection;
    private List<string> messages = new List<string>();
    private string? userInput;
    private string? messageInput;

    [Inject]
    private NavigationManager? NavigationManager { get; set; }

    [Inject]
    public IAccessTokenProvider? AccessTokenProvider { get; set; }

    protected override async Task OnInitializedAsync()
    {
        hubConnection = new HubConnectionBuilder()
            .WithUrl(NavigationManager!.ToAbsoluteUri("/chathub"), options =>
            {
                options.AccessTokenProvider = async () =>
                {
                    var accessTokenResult = await AccessTokenProvider!.RequestAccessToken();
                    accessTokenResult.TryGetToken(out var accessToken);
                    string token = accessToken.Value;
                    return token;
                };
            })
            .Build();

        hubConnection.On<string, string>("ReceiveMessage", (user, message) =>
        {
            var encodedMsg = $"{user}: {message}";
            messages.Add(encodedMsg);
            StateHasChanged();
        });
        await hubConnection.StartAsync();
    }

    private async Task Send()
    {
        if (hubConnection is not null)
        {
            await hubConnection.SendAsync("SendMessage", userInput, messageInput);
        }
    }

    public bool IsConnected => hubConnection?.State == HubConnectionState.Connected;

    public async ValueTask DisposeAsync()
    {
        if (hubConnection is not null)
        {
            await hubConnection.DisposeAsync();
        }
    }
}
