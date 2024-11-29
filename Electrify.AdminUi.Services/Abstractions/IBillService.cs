namespace Electrify.AdminUi.Services.Abstractions;

public interface IBillService
{
    Task<double?> GetBillForDay(Guid clientId, DateOnly date);
}