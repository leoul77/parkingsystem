namespace parkingSystem.Api.DTOs;

public class ActiveSessionDto
{
    public int TicketId { get; set; }
    public string TicketNumber { get; set; } = string.Empty;
    public string LicensePlate { get; set; } = string.Empty;
    public string DriverName { get; set; } = string.Empty;
    public string DriverPhone { get; set; } = string.Empty;
    public string SpotCode { get; set; } = string.Empty;
    public int FloorNumber { get; set; }
    public string SpotType { get; set; } = string.Empty;
    public string PricingPlan { get; set; } = string.Empty;
    public DateTime EntryTime { get; set; }
    public long ElapsedMinutes { get; set; }
    public decimal CurrentAccumulatedFee { get; set; }
}