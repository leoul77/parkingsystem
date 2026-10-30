using System;
using System.Collections.Generic;

namespace parkingSystem.Api.Models;

public partial class VwDailyCollection
{
    public DateOnly? CollectionDate { get; set; }

    public string CashierName { get; set; } = null!;

    public string PaymentMethod { get; set; } = null!;

    public long TransactionCount { get; set; }

    public decimal? TotalCollected { get; set; }
}
