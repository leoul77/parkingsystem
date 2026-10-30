using System;
using System.Collections.Generic;

namespace parkingSystem.Api.Models;

public partial class Driver
{
    public int DriverId { get; set; }

    public string FullName { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public string? Email { get; set; }

    public string? LicenseNumber { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<ParkingTicket> ParkingTickets { get; set; } = new List<ParkingTicket>();
}
