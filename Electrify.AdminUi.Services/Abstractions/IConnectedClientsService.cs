namespace Electrify.AdminUi.Services.Abstractions;

public interface IConnectedClientsService
{
    Task<IEnumerable<string>> GetConnectedClientIds();
}