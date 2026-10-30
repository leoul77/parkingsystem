using System;
using System.Collections.Generic;

namespace parkingSystem.Api.Models;

public partial class VwOccupancySummary
{
    public long TotalSpots { get; set; }

    public decimal? OccupiedSpots { get; set; }

    public decimal? AvailableSpots { get; set; }

    public decimal? OccupancyPercentage { get; set; }
}
