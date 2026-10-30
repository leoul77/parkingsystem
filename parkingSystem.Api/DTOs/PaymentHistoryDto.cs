namespace parkingSystem.Api.DTOs;

public class PaymentHistoryDto
{
    public int PaymentId { get; set; }
    public DateTime PaymentTime { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string CashierName { get; set; } = string.Empty;
    public string ReferenceNumber { get; set; } = string.Empty;
    public string TicketNumber { get; set; } = string.Empty;
    public string LicensePlate { get; set; } = string.Empty;
    public string DriverName { get; set; } = string.Empty;
    public string DriverPhone { get; set; } = string.Empty;
    public string SpotCode { get; set; } = string.Empty;
    public DateTime EntryTime { get; set; }
    public DateTime? ExitTime { get; set; }
    public int? TotalDurationMinutes { get; set; }
}