using Electrify.Models;
using Electrify.Models.Enums;
using Electrify.SmartMeterUi.Services.Abstractions;
using Microsoft.AspNetCore.Components;

namespace Electrify.SmartMeterUi.Components;

public partial class ClientSetup : ComponentBase
{
    [Inject] private IConnectionService ConnectionService { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] private IClientRegistrationService ClientRegistrationService { get; set; } = default!;

    private string _clientId = string.Empty;    
    private readonly List<ToastDefinition> _displayToasts = [];

    private async void HandleSetup()
    {
        bool clientIdExists = await ClientRegistrationService.ClientIdExists(Guid.Parse(_clientId));

        if (Guid.TryParse(_clientId, out Guid clientId) && clientIdExists)
        {
            ConnectionService.SetClientId(clientId);
            NavigationManager.NavigateTo("/smart-meter-home");
        }
        else
        {
            // Display error message
            _displayToasts.Add(new ToastDefinition
            {
                Title = "Invalid Client ID",
                Message = "Please enter a valid Client ID.",
                Type = ToastType.InvalidClientId,
            });
        }
    }
}
