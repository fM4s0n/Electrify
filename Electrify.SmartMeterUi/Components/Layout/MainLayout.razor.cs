using Electrify.SmartMeterUi.Services.Abstraction;
using Microsoft.AspNetCore.Components;

namespace Electrify.SmartMeterUi.Components.Layout;

public partial class MainLayout
{
    [Inject] private IConnectionService ConnectionService { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        // Make an initial connection to the server
        await ConnectionService.InitializeConnectionAsync(false);
    }
}


