using Gurux.DLMS.Enums;
using Gurux.Net;

namespace Electrify.Dlms.Options;

public sealed class DlmsClientOptions
{
    public bool UseLogicalNameReferencing { get; init; }
    public int ClientAddress { get; init; }
    public int ServerAddress { get; init; }
    public Authentication Authentication { get; init; }
    public string Password { get; init; } = string.Empty;
    public InterfaceType InterfaceType { get; init; }
    public NetworkType Protocol { get; init; }
    public string ServerHostname { get; init; } = string.Empty;
    public int ServerPort { get; init; }
    public string InvocationCounter { get; init; } = string.Empty;
}