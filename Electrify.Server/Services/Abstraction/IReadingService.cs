namespace Electrify.Server.Services.Abstraction;

public interface IReadingService
{
    public double GetReadingsForDay(Guid clientId, DateOnly date);
}