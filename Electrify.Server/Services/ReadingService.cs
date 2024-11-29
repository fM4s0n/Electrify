using Electrify.Server.Database;
using Electrify.Server.Services.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace Electrify.Server.Services;

public class ReadingService(ElectrifyDbContext dbContext, ILogger<ReadingService> logger) 
    : IReadingService
{
    public double GetReadingsForDay(Guid clientId, DateOnly date)
    {
        var readings = dbContext.Readings
            .Where(reading => reading.ClientId == clientId);
        
        if (readings.Any() == false)
        {
            logger.LogWarning("No readings found for client {clientId} on {date}", clientId, date);
            return 0.0;
        }
        
        double previousEnergyUsage = dbContext.Readings
            .Where(reading => reading.ClientId == clientId && DateOnly.FromDateTime(reading.DateTime) < date)
            .OrderByDescending(reading => reading.DateTime)
            .FirstOrDefault()?
            .EnergyUsage ?? 0;
        
        double usageCostCalculated = previousEnergyUsage;
        double totalCost = 0.0;
        
        foreach (var reading in readings)
        {
            var readingCost = (reading.EnergyUsage - usageCostCalculated) * reading.Tariff;
            totalCost += readingCost;
            usageCostCalculated = reading.EnergyUsage;
        }

        return totalCost;
    }
}