namespace parkingSystem.Api.DTOs;

public class CheckOutRequest
{
    public string TicketNumber { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = "CASH";
    public string CashierName { get; set; } = "Cashier";
}