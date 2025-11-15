using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
namespace ComponentLib;

public partial class PopupDialog : IDisposable
{
    [Inject]
    public required DialogService Service { get; set; }

    [Parameter]
    public required IJSRuntime JSRuntime { get; set; }

    List<Dialog> dialogs = [];

    protected override void OnInitialized()
    {
        Service.OnOpen += OnOpen;
        Service.OnClose += OnClose;
    }

    public void Dispose()
    {
        Service.OnOpen -= OnOpen;
        Service.OnClose -= OnClose;
    }

    void OnOpen(string title, Type type, Dictionary<string, object> parameters, DialogOptions options)
    {
        Open(title, type, parameters, options).ConfigureAwait(false);
    }

    public async Task Open(string title, Type type, Dictionary<string, object> parameters, DialogOptions options)
    {
        dialogs.Add(new Dialog() { Title = title, Type = type, Parameters = parameters, Options = options });

        await InvokeAsync(StateHasChanged);
    }

    void OnClose(dynamic result)
    {
        Close(result).ConfigureAwait(false);
    }

    public async Task Close(dynamic result)
    {
        var lastDialog = dialogs.LastOrDefault();
        if (lastDialog != null)
        {
            dialogs.Remove(lastDialog);
            // if (dialogs.Count == 0) await JSRuntime.InvokeAsync<string>("Radzen.closeDialog");
        }

        await InvokeAsync(StateHasChanged);
    }
}
