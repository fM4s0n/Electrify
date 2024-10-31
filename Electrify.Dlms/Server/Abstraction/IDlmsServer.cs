using Electrify.Dlms.Options;
using Gurux.DLMS.Objects;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace Electrify.Dlms.Server.Abstraction;

public interface IDlmsServer : IDisposable
{
    void AddRegister(GXDLMSRegister register);

    double GetEnergy();

    void SetEnergy(int energyValue);

    void Initialise(IOptions<DlmsServerOptions> options, TraceLevel traceLevel);
}