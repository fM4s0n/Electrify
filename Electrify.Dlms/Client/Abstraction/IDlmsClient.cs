using Electrify.Models.Models;

namespace Electrify.Dlms.Client.Abstraction;

public interface IDlmsClient
{
    public IEnumerable<Reading> ReadEnergyProfile(DateTime sinceTime);
    void WriteValueToRegister(string logicalName, object value);
}