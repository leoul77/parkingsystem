using System;
using System.Collections.Generic;

namespace parkingSystem.Api.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int TicketId { get; set; }

    public decimal Amount { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public string? CashierName { get; set; }

    public string PaymentStatus { get; set; } = null!;

    public DateTime PaymentTime { get; set; }

    public string? ReferenceNumber { get; set; }

    public virtual ParkingTicket Ticket { get; set; } = null!;
}
