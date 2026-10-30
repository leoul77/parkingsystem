using parkingSystem.Api.DTOs;

namespace parkingSystem.Api.Services;

public interface IParkingService
{
    Task<List<OccupancySummaryDto>> GetOccupancySummaryAsync();
    Task<List<ActiveSessionDto>> GetActiveSessionsAsync();
    Task<string> CheckInAsync(CheckInRequest request);
    Task<CheckOutResponse> CheckOutAsync(CheckOutRequest request);
    Task<List<PaymentHistoryDto>> GetPaymentHistoryAsync();
    Task<List<DailyCollectionDto>> GetDailyCollectionsAsync();
    Task<decimal> GetTodayTotalAsync();
    Task<decimal> GetTotalRevenueAsync();
    Task<PricingConfigResponse> GetPricingConfigAsync();
    Task SavePricingConfigAsync(PricingConfigRequest request);
}