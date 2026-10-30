namespace parkingSystem.Api.DTOs;

public class DailyCollectionDto
{
    public DateTime CollectionDate { get; set; }
    public string CashierName { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;
    public long TransactionCount { get; set; }
    public decimal TotalCollected { get; set; }
}