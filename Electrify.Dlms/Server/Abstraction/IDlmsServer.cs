using System.Diagnostics;
using Electrify.Dlms.Options;
using Gurux.DLMS.Enums;
using Gurux.DLMS.Objects;
using Microsoft.Extensions.Options;

namespace Electrify.Dlms.Server.Abstraction;

public interface IDlmsServer : IDisposable
{
    void AddObject(GXDLMSObject dlmsObject, AccessMode3 valueAccessMode = AccessMode3.Read);

    void SetEnergy(int energyValue);

    void Initialise(IOptions<DlmsServerOptions> options, Action onConnectedCallback, Action onDisconnectedCallback);
}