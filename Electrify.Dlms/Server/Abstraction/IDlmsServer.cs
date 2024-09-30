using Gurux.DLMS.Objects;

namespace Electrify.Dlms.Server.Abstraction;

public interface IDlmsServer : IDisposable
{
    void AddRegister(GXDLMSRegister register);

    double GetEnergy();

    void SetEnergy(int energyValue);
}