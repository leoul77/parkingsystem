using System;
using System.Collections.Generic;

namespace parkingSystem.Api.Models;

public partial class Vehicle
{
    public int VehicleId { get; set; }

    public string LicensePlate { get; set; } = null!;

    public string VehicleType { get; set; } = null!;

    public string? MakeModel { get; set; }

    public string? Color { get; set; }

    public DateTime? CreatedAt { get; set; }
}
