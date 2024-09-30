namespace Electrify.Dlms.Client.Abstraction;

public interface IDlmsClient
{
    /// <summary>
    /// Initiates a read operation for Energy from the DLMS Server.
    /// </summary>
    public void ReadEnergy();
}