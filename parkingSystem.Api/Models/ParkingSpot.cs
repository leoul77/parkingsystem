using System;
using System.Collections.Generic;

namespace parkingSystem.Api.Models;

public partial class ParkingSpot
{
    public int SpotId { get; set; }

    public string SpotCode { get; set; } = null!;

    public int FloorNumber { get; set; }

    public string SpotType { get; set; } = null!;

    public int? CustomRateId { get; set; }

    public bool IsOccupied { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual PricingRate? CustomRate { get; set; }

    public virtual ICollection<ParkingTicket> ParkingTickets { get; set; } = new List<ParkingTicket>();
}
