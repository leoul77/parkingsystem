namespace parkingSystem.Api.DTOs;

public class PricingConfigResponse
{
    public bool PricingConfigured { get; set; }
    public string PlanName { get; set; } = string.Empty;
    public decimal FirstHourRate { get; set; }
    public decimal AdditionalHourlyRate { get; set; }
    public int GracePeriodMinutes { get; set; }
}