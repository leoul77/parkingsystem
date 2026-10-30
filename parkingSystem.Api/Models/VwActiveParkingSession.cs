using System;
using System.Collections.Generic;

namespace parkingSystem.Api.Models;

public partial class VwActiveParkingSession
{
    public int TicketId { get; set; }

    public string TicketNumber { get; set; } = null!;

    public string LicensePlate { get; set; } = null!;

    public string? DriverName { get; set; }

    public string? DriverPhone { get; set; }

    public string SpotCode { get; set; } = null!;

    public int FloorNumber { get; set; }

    public string SpotType { get; set; } = null!;

    public string PricingPlan { get; set; } = null!;

    public DateTime EntryTime { get; set; }

    public long? ElapsedMinutes { get; set; }

    public decimal? CurrentAccumulatedFee { get; set; }
}
