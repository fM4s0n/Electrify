using Electrify.AdminUi.Services.Abstractions;
using Microsoft.AspNetCore.Components;

namespace Electrify.AdminUi.Components.Pages;

public partial class Bill : ComponentBase
{
    [Inject]
    private IBillService BillService { get; set; } = default!;
    
    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;
    
    [Parameter]
    public Guid ClientId { get; set; }
    
    private double? _payableAmount;

    protected override async Task OnParametersSetAsync()
    {
        _payableAmount = await BillService.GetBillForDay(ClientId, DateOnly.FromDateTime(DateTime.Now));
    }
}