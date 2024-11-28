using Electrify.AdminUi.Services.Abstractions;
using Electrify.Server.ApiClient.Abstraction;

namespace Electrify.AdminUi.Services;

public class BillService(IElectrifyApiClient electrifyApiClient) : IBillService
{
    public async Task<double?> GetBillForDay(Guid clientId, DateOnly date)
    {
        return await electrifyApiClient.GetClientBill(clientId.ToString(), date);
    }
}