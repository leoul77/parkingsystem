using System;
using System.Collections.Generic;

namespace parkingSystem.Api.Models;

public partial class VwPaymentHistory
{
    public int PaymentId { get; set; }

    public DateTime PaymentTime { get; set; }

    public decimal Amount { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public string? CashierName { get; set; }

    public string? ReferenceNumber { get; set; }

    public string TicketNumber { get; set; } = null!;

    public string LicensePlate { get; set; } = null!;

    public string? DriverName { get; set; }

    public string? DriverPhone { get; set; }

    public string SpotCode { get; set; } = null!;

    public DateTime EntryTime { get; set; }

    public DateTime? ExitTime { get; set; }

    public int? TotalDurationMinutes { get; set; }
}
