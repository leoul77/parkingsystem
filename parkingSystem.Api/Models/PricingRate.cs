using System;
using System.Collections.Generic;

namespace parkingSystem.Api.Models;

public partial class PricingRate
{
    public int RateId { get; set; }

    public string Name { get; set; } = null!;

    public string VehicleType { get; set; } = null!;

    public int GracePeriodMins { get; set; }

    public decimal FirstHourRate { get; set; }

    public decimal AdditionalHourlyRate { get; set; }

    public decimal MaxDailyRate { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<ParkingSpot> ParkingSpots { get; set; } = new List<ParkingSpot>();

    public virtual ICollection<ParkingTicket> ParkingTickets { get; set; } = new List<ParkingTicket>();
}
