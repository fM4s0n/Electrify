namespace Electrify.Dlms.Client.Abstraction;

public interface IDlmsClient
{
    /// <summary>
    /// Initiates a read operation for Energy from the DLMS Server.
    /// </summary>
    public void ReadEnergy();
    
    /// <summary>
    /// Initiates a write operation for Energy to the DLMS Server.
    /// </summary>
    /// <param name="energyValue"></param>
    public void SetEnergy(int energyValue);
}