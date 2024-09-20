namespace Electrify.Models.Models;

public class Reading
{
    public Reading() 
    {
    }

    public required int Id { get; set; }
    public required int ClientId { get; set; }
    public required string MeterReading { get; set; }
}
