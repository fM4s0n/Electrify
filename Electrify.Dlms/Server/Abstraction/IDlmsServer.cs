using System.Diagnostics;
using Electrify.Dlms.Options;
using Gurux.DLMS.Objects;
using Microsoft.Extensions.Options;

namespace Electrify.Dlms.Server.Abstraction;

public interface IDlmsServer : IDisposable
{
    void AddObject(GXDLMSObject dlmsObject, bool writeAccess = false);

    void SetEnergy(int energyValue);

    void Initialise(IOptions<DlmsServerOptions> options, TraceLevel traceLevel);
}