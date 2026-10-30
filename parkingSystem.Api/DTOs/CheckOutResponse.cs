namespace parkingSystem.Api.DTOs;

public class CheckOutResponse
{
    public string TicketNumber { get; set; } = string.Empty;
    public string LicensePlate { get; set; } = string.Empty;
    public string DriverName { get; set; } = string.Empty;
    public string SpotCode { get; set; } = string.Empty;
    public DateTime EntryTime { get; set; }
    public DateTime ExitTime { get; set; }
    public int DurationMinutes { get; set; }
    public decimal TotalAmount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string CashierName { get; set; } = string.Empty;
    public string ReferenceNumber { get; set; } = string.Empty;
    public string Breakdown { get; set; } = string.Empty;
}