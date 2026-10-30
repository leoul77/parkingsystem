namespace parkingSystem.Api.DTOs;

public class OccupancySummaryDto
{
    public long TotalSpots { get; set; }
    public decimal OccupiedSpots { get; set; }
    public decimal AvailableSpots { get; set; }
    public decimal OccupancyPercentage { get; set; }
}