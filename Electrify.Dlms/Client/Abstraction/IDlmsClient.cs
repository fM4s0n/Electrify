using Electrify.Models;

namespace Electrify.Dlms.Client.Abstraction;

public interface IDlmsClient
{
    public IEnumerable<Reading> ReadEnergyProfile(DateTime sinceTime);

    void WriteTariff(double tariff);
}