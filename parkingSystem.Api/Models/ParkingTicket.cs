using System;
using System.Collections.Generic;

namespace parkingSystem.Api.Models;

public partial class ParkingTicket
{
    public int TicketId { get; set; }

    public string TicketNumber { get; set; } = null!;

    public string LicensePlate { get; set; } = null!;

    public int? DriverId { get; set; }

    public int SpotId { get; set; }

    public int RateId { get; set; }

    public DateTime EntryTime { get; set; }

    public DateTime? ExitTime { get; set; }

    public int? TotalDurationMinutes { get; set; }

    public decimal? TotalAmount { get; set; }

    public string Status { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Driver? Driver { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual PricingRate Rate { get; set; } = null!;

    public virtual ParkingSpot Spot { get; set; } = null!;
}
