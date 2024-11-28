using Electrify.Models;

namespace Electrify.Dlms.Client.Abstraction;

public interface IDlmsClient
{
    public Guid ClientId { get; }
    public IEnumerable<Reading> ReadEnergyProfile(DateTime sinceTime);

    void WriteTariff(double tariff);

    void WriteErrorMessage(string message);
}