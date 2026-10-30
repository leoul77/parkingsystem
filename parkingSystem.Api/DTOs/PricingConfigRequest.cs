namespace parkingSystem.Api.DTOs;

public class PricingConfigRequest
{
    public decimal FirstHourRate { get; set; }
    public decimal AdditionalHourlyRate { get; set; }
    
    public int GracePeriodMinutes { get; set; } = 15;
}